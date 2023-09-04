using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoStarterProject.EventsHandler
{
	public class GenericHandler : INotificationHandler<ContentSavingNotification>
	{
		private readonly IUmbracoContextAccessor _umbracoContextAccessor;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IConfiguration _config;
		public GenericHandler(IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor, IConfiguration config)
		{
			_umbracoContextAccessor = umbracoContextAccessor;
			_variationContextAccessor = variationContextAccessor;
			_config = config;
		}


		public void Handle(ContentSavingNotification notification)
		{

			var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();

			var culture = _variationContextAccessor.VariationContext.Culture;
			foreach (var node in notification.SavedEntities)
			{
				var mediaImages = node.Properties.Where(x => x.PropertyType.PropertyEditorAlias == "Umbraco.MediaPicker3");
				var relatedLinks = node.Properties.Where(x => x.PropertyType.PropertyEditorAlias == "Umbraco.MultiNodeTreePicker");
				var mediaInUploader = node.Properties.Where(x => x.PropertyType.PropertyEditorAlias == "Umbraco.UploadField");
				foreach (var media in mediaImages)
				{
					var mediaImage = media?.GetValue(culture)?.ToString();

					if (mediaImage != null)
					{
						JArray imageProps = mediaImage != null ? JArray.Parse(mediaImage.ToString()) : null;
						if (imageProps != null)
						{
							foreach (var item in imageProps)
							{
								JObject jObject = JObject.Parse(item.ToString());
								var key = Guid.Parse(((JValue)jObject["mediaKey"]).Value.ToString());
								var image = umbracoContext.Media.GetById(true, key); //get image by key here
								if (image != null && image.ContentType.Alias.Equals("Image"))
								{
									var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image)?.UmbracoBytes;
									int.TryParse(_config["GenericListingMediaMaxSize"].ToString(), out int maxImageSize);

									var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image).MediaUrl().GetFileExtension();
									if (!HasImageAllowedExtension(fileExtension))
									{
										notification.CancelOperation(new EventMessage("Error",
											$"Image with not allowed extensions,Allowed extensions are PNG, JPG, JPE, MPO, JPEG ",
											   EventMessageType.Error));
										break;
									}
									if (fileLength > maxImageSize * 1024 * 1024)//5MB
									{
										notification.CancelOperation(new EventMessage("Error",
													$"Max size for " + media.Alias + " field is " + maxImageSize + "MB",
													EventMessageType.Error));
										break;
									}
								}
								else if (image != null && image.ContentType.Alias.Equals("umbracoMediaVideo"))
								{
									var video = umbracoContext.Media.GetById(true, key); //get image by key here
									if (video != null)
									{
										var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVideo)video).UmbracoBytes;
										int.TryParse(_config["GenericListingMediaMaxSize"].ToString(), out int maxVideoSize);
										var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVideo)video).MediaUrl().GetFileExtension();
										if (!HasVideoAllowedExtension(fileExtension))
										{
											notification.CancelOperation(new EventMessage("Error",
												$"Video with not allowed extensions,Allowed extensions are MP4",
												   EventMessageType.Error));
											break;
										}
										if (fileLength > maxVideoSize * 1024 * 1024)//30MB
										{
											notification.CancelOperation(new EventMessage("Error",
														$"Max size for video " + "" + maxVideoSize + "MB",
														EventMessageType.Error));
											break;
										}
									}
								}
							}
						}
					}
					else if (mediaImage != null)
					{
						JArray imageProps = mediaImage != null ? JArray.Parse(mediaImage.ToString()) : null;
						if (imageProps != null)
						{
							foreach (var item in imageProps)
							{
								JObject jObject = JObject.Parse(item.ToString());
								var key = Guid.Parse(((JValue)jObject["mediaKey"]).Value.ToString());
								var image = umbracoContext.Media.GetById(true, key); //get image by key here
								if (image != null && image.ContentType.Alias.Equals("Image"))
								{
									var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image)?.UmbracoBytes;
									int.TryParse(_config["MainImageMaxSize"].ToString(), out int maxImageSize);

									var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image).MediaUrl().GetFileExtension();
									if (!HasImageAllowedExtension(fileExtension))
									{
										notification.CancelOperation(new EventMessage("Error",
											$"Image with not allowed extensions,Allowed extensions are PNG, JPG, GIF, JPE, MPO, JPEG ",
											   EventMessageType.Error));
										break;
									}

									if (fileLength > maxImageSize * 1024 * 1024)//5MB
									{
										notification.CancelOperation(new EventMessage("Error",
													$"Max size for " + media.Alias + " field is " + maxImageSize + "MB",
													EventMessageType.Error));
										break;
									}

								}

							}
						}
					}
				}
				if (relatedLinks != null)
				{

					foreach (var link in relatedLinks)
					{
					
						
						var relatedLink = link?.GetValue(culture)?.ToString();
						if (!string.IsNullOrEmpty(relatedLink))
						{
							var ItemsUdiList = relatedLink.Split(',').ToList();
							foreach (var udi in ItemsUdiList)
							{
								var guid = Guid.ParseExact(udi.Substring(udi.Length - 32), "N");
								var PageByGuid = umbracoContext.Content.GetById(guid);
								if (PageByGuid == null)
								{
									notification.CancelOperation(new EventMessage("Error",
										"You should not choose an item of content that is currently unpublished !",
											EventMessageType.Error));
									break;
								}
								else
								{
								
									if (!PageByGuid.IsPublished())
									{
										notification.CancelOperation(new EventMessage("Error",
										"You should not choose an item of content that is currently unpublished !",
											EventMessageType.Error));
										break;

									}
								}
							}
							
						}

					}

				}
                /////////////////////////////////////////////////////////////////////////////////////////////////
                ////check BlockList in any node prop
                foreach (var property in node.Properties)
                {
                    if (property.PropertyType.PropertyEditorAlias == "Umbraco.BlockList")
                    {
                        var BlockItems = property?.GetValue(culture)?.ToString();
                        CheckItemsInBlockList(BlockItems, umbracoContext, notification, culture);

                    }
                }
            }
		}

        #region work into BlockList while saving 
        //function that cheeck if there is any items of type blocklist in main blocklist and validate inner items.
        private void CheckItemsInBlockList(string BlockItems, IUmbracoContext context, ContentSavingNotification conNotification, string culture)
        {
            if (!string.IsNullOrEmpty(BlockItems))
            {
                var jObject = JObject.Parse(BlockItems);
                JArray contentData = (JArray)jObject["contentData"]!;

                if (contentData != null)
                {
                    foreach (var token in contentData)
                    {

                        JObject galleryObject = JObject.Parse(token.ToString());
                        //Check that BlockList contain any blockList
                        foreach (var prop in galleryObject.Properties())
                        {
                            if (prop.Value.Type == JTokenType.String)
                            {
                                try
                                {
                                    JObject propertyValue = JObject.Parse(prop.Value.ToString());

                                    // Check if the property value is a block list
                                    if (propertyValue.ContainsKey("layout") && propertyValue["layout"]["Umbraco.BlockList"] != null)
                                    {

                                        //Recursion
                                        CheckItemsInBlockList(propertyValue.ToString(), context, conNotification, culture);
                                    }
                                }
                                catch (Exception)
                                {
                                    // Not a valid JSON object, continue to the next property
                                    continue;
                                }
                            }


                        }

                        bool res = ValidateImageAndVideos(galleryObject, conNotification, context);
                        if (!res)
                            break;


                    }

                }
            }
        }

        //validate
        public bool ValidateImageAndVideos(JObject galleryObject, ContentSavingNotification conNotification, IUmbracoContext context)
        {
            /*//TODO:
                foreach (var property in galleryObject.Properties())
                {
                    try
                    {
                        var arr = JArray.Parse(property.Value.ToString())[0].ToList();
                        var keyy = arr[0]?.First?.ToString();
                        var mediaKeyy = arr[1]?.First?.ToString();

                        var imageObject = mediaKeyy;
                        var thumbnailObject = galleryObject?.GetValue("videoThumbnail")?.ToString();
                        var mediaObject = GetMediaString(imageObject, thumbnailObject);

                        var videoObject = galleryObject.GetValue("mediaVideoUploader")?.ToString();
                        var videoResult = videoObject != null && videoObject.Length > 0 ? videoObject?.Substring(1, videoObject.Length - 2) : "";
                        var videoKey = !string.IsNullOrEmpty(videoResult) ? JObject.Parse(videoResult?.ToString())?.GetValue("mediaKey")?.ToString() : "";

                        if (!string.IsNullOrEmpty(mediaObject))
                        {
                            var key = Guid.Parse(mediaObject);
                            var image = context.Media.GetById(true, key); //get image by key here
                            if (image != null)
                            {
                                var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image).UmbracoBytes;
                                int.TryParse(_config["MainImageMaxSize"].ToString(), out int maxImageSize);

                                var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image).MediaUrl().GetFileExtension();
                                if (!HasImageAllowedExtension(fileExtension))
                                {
                                    conNotification.CancelOperation(new EventMessage("Error",
                                        $"Image with not allowed extensions,Allowed extensions are PNG, JPG, JPEG, MPO",
                                           EventMessageType.Error));
                                    return false;
                                }

                                if (fileLength > maxImageSize * 1024 * 1024)//5MB
                                {
                                    conNotification.CancelOperation(new EventMessage("Error",
                                                $"Max size for image and thumbnails " + "" + maxImageSize + "MB",
                                                EventMessageType.Error));
                                    return false;
                                }

                            }

                        }
                        if (!string.IsNullOrEmpty(videoKey))
                        {
                            var key = Guid.Parse(videoKey);
                            var video = context.Media.GetById(true, key); //get image by key here
                            if (video != null)
                            {
                                var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVideo)video).UmbracoBytes;
                                int.TryParse(_config["MainVideoMaxSize"].ToString(), out int maxVideoSize);

                                var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVideo)video).MediaUrl().GetFileExtension();
                                if (!HasVideoAllowedExtension(fileExtension))
                                {
                                    conNotification.CancelOperation(new EventMessage("Error",
                                        $"Video with not allowed extensions,Allowed extensions are MP4",
                                           EventMessageType.Error));
                                    return false;
                                }

                                if (fileLength > maxVideoSize * 1024 * 1024)//30MB
                                {
                                    conNotification.CancelOperation(new EventMessage("Error",
                                                $"Max size for video " + "" + maxVideoSize + "MB",
                                                EventMessageType.Error));
                                    return false;
                                }

                            }
                        }

                    }
                    catch { continue; }





                }
                return true;*/

            var imageObject = galleryObject?.GetValue("mediaImage")?.ToString();
            var thumbnailObject = galleryObject?.GetValue("videoThumbnail")?.ToString();
            var mediaObject = GetMediaString(imageObject, thumbnailObject);
            var result = mediaObject != null && mediaObject.Length > 0 ? mediaObject?.Substring(1, mediaObject.Length - 2) : "";
            var imageKey = !string.IsNullOrEmpty(result) ? JObject.Parse(result?.ToString())?.GetValue("mediaKey")?.ToString() : "";

            var videoObject = galleryObject.GetValue("mediaVideoUploader")?.ToString();
            var videoResult = videoObject != null && videoObject.Length > 0 ? videoObject?.Substring(1, videoObject.Length - 2) : "";
            var videoKey = !string.IsNullOrEmpty(videoResult) ? JObject.Parse(videoResult?.ToString())?.GetValue("mediaKey")?.ToString() : "";

            if (!string.IsNullOrEmpty(imageKey))
            {
                var key = Guid.Parse(imageKey);
                var image = context.Media.GetById(true, key); //get image by key here
                if (image != null)
                {
                    var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image).UmbracoBytes;
                    int.TryParse(_config["MainImageMaxSize"].ToString(), out int maxImageSize);

                    var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.Image)image).MediaUrl().GetFileExtension();
                    if (!HasImageAllowedExtension(fileExtension))
                    {
                        conNotification.CancelOperation(new EventMessage("Error",
                            $"Image with not allowed extensions,Allowed extensions are PNG, JPG, JPEG, MPO",
                               EventMessageType.Error));
                        return false;
                    }

                    if (fileLength > maxImageSize * 1024 * 1024)//5MB
                    {
                        conNotification.CancelOperation(new EventMessage("Error",
                                    $"Max size for image and thumbnails " + "" + maxImageSize + "MB",
                                    EventMessageType.Error));
                        return false;
                    }

                }

            }
            if (!string.IsNullOrEmpty(videoKey))
            {
                var key = Guid.Parse(videoKey);
                var video = context.Media.GetById(true, key); //get image by key here
                if (video != null)
                {
                    var fileLength = ((Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVideo)video).UmbracoBytes;
                    int.TryParse(_config["MainVideoMaxSize"].ToString(), out int maxVideoSize);

                    var fileExtension = ((Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVideo)video).MediaUrl().GetFileExtension();
                    if (!HasVideoAllowedExtension(fileExtension))
                    {
                        conNotification.CancelOperation(new EventMessage("Error",
                            $"Video with not allowed extensions,Allowed extensions are MP4",
                               EventMessageType.Error));
                        return false;
                    }

                    if (fileLength > maxVideoSize * 1024 * 1024)//30MB
                    {
                        conNotification.CancelOperation(new EventMessage("Error",
                                    $"Max size for video " + "" + maxVideoSize + "MB",
                                    EventMessageType.Error));
                        return false;
                    }

                }
            }
            return true;
        }

        #endregion



        private bool HasImageAllowedExtension(string fileExtension)
		{
			string[] allowedExtensions = new string[] { ".PNG", ".JPG", ".MPO", ".JPEG", ".png", ".jpg", ".mpo", ".jpeg" };
			return allowedExtensions.Contains(fileExtension);
		}
		private bool HasVideoAllowedExtension(string fileExtension)
		{
			string[] allowedExtensions = new string[] { ".MP4", ".mp4" };
			return allowedExtensions.Contains(fileExtension);
		}

		private string GetMediaString(string imageObject, string thumbnailObject)
		{
			if (!string.IsNullOrEmpty(imageObject))
				return imageObject;
			else if (!string.IsNullOrEmpty(thumbnailObject))
				return thumbnailObject;
			return string.Empty;
		}

	}
}
