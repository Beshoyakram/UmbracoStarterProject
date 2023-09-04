<template>
    <div class="social-media">
        <div class="container">
            <div class="row">
                <div class="col">
                    <div>شارك</div>
                    <ShareNetwork network="facebook"
                                  key="facebook"
                                  :url="fullUrl"
                                  :title="sharing.title"
                                  :description="sharing.description"
                                  @click="sendAPICallSocialMedia('Facebook')">
                        <i class="icon-facebook"></i>
                    </ShareNetwork>

                    <ShareNetwork network="twitter"
                                  key="twitter"
                                  :url="shortenUrl"
                                  :title="sharing.title"
                                  :description="sharing.description"
                                  :twitterUser="sharing.twitterUser"
                                  @click="sendAPICallSocialMedia('Twitter')">
                        <i class="icon-twitter"></i>
                    </ShareNetwork>

                    <ShareNetwork network="WhatsApp"
                                  key="WhatsApp"
                                  :url="fullUrl"
                                  :title="sharing.title"
                                  :description="sharing.description"
                                  @click="sendAPICallSocialMedia('WhatsApp')">
                        <i class="icon-whatsapp"></i>
                    </ShareNetwork>

                    <a class="share-network-twitter share-network-copy-link"
                       href="javascript:void(0)"
                       @click="copyLink()">
                        <i class="icon-attachment"></i>
                    </a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import axios from "axios";
    import copy from "copy-to-clipboard";
    let pageURL = window.location.href;
    let socialThis;

    export default {
        name: "socialMediaSharing",
        data() {
            return {
                props: {},
                sharing: {
                    url: "",
                    title: "",
                    description: "",
                },
                shortenUrl: "",
                fullUrl: "",
            };
        },
        mounted() {
            socialThis = this;
            this.props = this.$attrs.props;

            socialThis.sharing.title = this.$attrs.props.titleToShareOnSocial;
            socialThis.sharing.description = this.$attrs.props.descriptionToShareOnSocial;
            socialThis.fullUrl = pageURL;
            axios
                .get(`https://tinyurl.com/api-create.php?url=${pageURL}`)
                .then((res) => {
                    this.shortenUrl = res.data;
                })
                .catch(() => {
                    socialThis.shortenUrl = pageURL;
                });
        },
        methods: {
            copyLink() {
                if (document.body.classList.contains("body--ipad")) {
                    //use 'copy to clipboard' package in ipad only
                    copy(pageURL);
                } else {
                    navigator.clipboard.writeText(`${pageURL}`);
                }
                document.querySelector(
                    "#snackbar"
                ).innerText = this.$attrs.props.linkCopiedSuccessfully;
                document.querySelector("#snackbar").classList.add("show");
                setTimeout(function () {
                    document.querySelector("#snackbar").classList.remove("show");
                }, 3000);
                this.sendAPICallSocialMedia('CopyLink')
            },
            sendAPICallSocialMedia(socialMediaType) {
                axios
                    .get(`${window.location.origin}/umbraco/api/Sharing/Visit?PageId=${this.props.pageID}&LinkName=${socialMediaType}&Culture=${this.props.Culture}`)
                    .then((res) => {
                        this.shortenUrl = res.data;
                    })
                    .catch((err) => {
                        console.log(err);
                    });
            }
        },
    };
</script>
