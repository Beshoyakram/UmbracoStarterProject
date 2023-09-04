<template>
  <transition name="cool-lightbox-modal">
    <div
      class="cool-lightbox"
      v-bind:class="lightboxClasses"
      v-if="isVisible"
      ref="coolLightbox"
      @click="closeModal"
      v-bind:style="lightboxStyles"
    >
      <div v-if="gallery" class="cool-lightbox-thumbs">
        <div class="cool-lightbox-thumbs__list">
          <button
            type="button"
            v-for="(item, itemIndex) in items"
            :key="itemIndex"
            :class="{
              active: itemIndex === imgIndex,
              'is-video': getMediaType(itemIndex) === 'video',
            }"
            @click="imgIndex = itemIndex"
            class="cool-lightbox__thumb"
          >
            <svg
              class="cool-lightbox__thumb__icon"
              xmlns="http://www.w3.org/2000/svg"
              v-if="getMediaType(itemIndex) === 'video'"
              viewBox="0 0 24 24"
            >
              <path d="M6.5 5.4v13.2l11-6.6z"></path>
            </svg>

            <img :src="itemThumb(getItemSrc(itemIndex), itemIndex)" alt="" />
          </button>
        </div>
      </div>

      <div
        class="cool-lightbox__inner"
        :style="innerStyles"
        @mousedown="startSwipe"
        @mousemove="continueSwipe"
        @mouseup="endSwipe"
        @touchstart="startSwipe"
        @touchmove="continueSwipe"
        @touchend="endSwipe"
      >
        <div class="cool-lightbox__progressbar" :style="stylesInterval"></div>
        <div class="cool-lightbox__navigation">
          <button
            type="button"
            class="cool-lightbox-button cool-lightbox-button--prev"
            title="Previous"
            :class="buttonsClasses"
            v-show="(hasPreviousButton || loopData) && items.length > 1"
            @click="onPrevClick"
          >
            <slot name="icon-previous">
              <div class="cool-lightbox-button__icon">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                  <path
                    d="M11.28 15.7l-1.34 1.37L5 12l4.94-5.07 1.34 1.38-2.68 2.72H19v1.94H8.6z"
                  ></path>
                </svg>
              </div>
            </slot>
          </button>

          <button
            type="button"
            class="cool-lightbox-button cool-lightbox-button--next"
            title="Next"
            :class="buttonsClasses"
            v-show="(hasNextButton || loopData) && items.length > 1"
            @click="onNextClick(false)"
          >
            <slot name="icon-next">
              <div class="cool-lightbox-button__icon">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                  <path
                    d="M15.4 12.97l-2.68 2.72 1.34 1.38L19 12l-4.94-5.07-1.34 1.38 2.68 2.72H5v1.94z"
                  ></path>
                </svg>
              </div>
            </slot>
          </button>
        </div>
        <!--/cool-lightbox__navigation-->
        <div
          v-if="effect === 'swipe'"
          class="cool-lightbox__wrapper cool-lightbox__wrapper--swipe"
          :style="{
            transform:
              'translate3d(' +
              xSwipeWrapper +
              'px, ' +
              ySwipeWrapper +
              'px, 0)',
            transition: swipeAnimation,
          }"
        >
          <div
            v-for="(item, itemIndex) in items"
            :key="itemIndex"
            :ref="setItemsRef"
            class="cool-lightbox__slide"
            :class="{ 'cool-lightbox__slide--current': itemIndex === imgIndex }"
          >
            <div
              v-lazyload
              v-if="getMediaType(itemIndex) === 'image'"
              key="image"
              :style="imgWrapperStyle"
              class="cool-lightbox__slide__img"
            >
              <img
                v-if="!isItemPicture(itemIndex)"
                :data-src="getItemSrc(itemIndex)"
                :data-srcset="getItemSrcSet(itemIndex)"
                :data-sizes="getItemSizes(itemIndex)"
                :key="itemIndex"
                draggable="false"
                :alt="getItemAlt(itemIndex)"
                @load="imageLoaded"
                @click="zoomImage(itemIndex)"
                @mousedown="handleMouseDown($event)"
                @mouseup="handleMouseUp($event)"
                @mousemove="handleMouseMove($event)"
                @touchstart="handleMouseDown($event)"
                @touchmove="handleMouseMove($event)"
                @touchend="handleMouseUp($event)"
              />
              <picture :key="itemIndex + 'pic'" v-else>
                <source
                  v-for="(source, sourceIndex) in getPictureSources(itemIndex)"
                  :data-srcset="source.srcset"
                  :data-media="source.media"
                  :type="source.type"
                  :data-sizes="source.sizes || getItemSizes(imgIndex)"
                  :key="`source-${imgIndex}-${sourceIndex}`"
                />
                <img
                  :data-src="getItemSrc(itemIndex)"
                  :data-srcset="getItemSrcSet(itemIndex)"
                  :data-sizes="getItemSizes(itemIndex)"
                  draggable="false"
                  :alt="getItemAlt(itemIndex)"
                  @load="imageLoaded"
                  @click="zoomImage(itemIndex)"
                  @mousedown="handleMouseDown($event)"
                  @mouseup="handleMouseUp($event)"
                  @mousemove="handleMouseMove($event)"
                  @touchstart="handleMouseDown($event)"
                  @touchmove="handleMouseMove($event)"
                  @touchend="handleMouseUp($event)"
                />
              </picture>

              <div v-show="imageLoading" class="cool-lightbox-loading-wrapper">
                <slot name="loading">
                  <div class="cool-lightbox-loading"></div>
                </slot>
              </div>
            </div>
            <div v-else key="video" class="cool-lightbox__iframe">
              <iframe
                class="cool-lightbox-video"
                v-autoplayObserver
                :data-autoplay="setAutoplay(itemIndex)"
                :src="getVideoUrl(getItemSrc(itemIndex))"
                v-if="
                  !checkIsMp4(getItemSrc(itemIndex)) &&
                    getMediaType(itemIndex) === 'video'
                "
                :style="aspectRatioVideo"
                :key="itemIndex"
                frameborder="0"
                allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                allowfullscreen
              >
              </iframe>

              <iframe
                class="cool-lightbox-pdf"
                :src="getItemSrc(itemIndex)"
                v-if="
                  getMediaType(itemIndex) === 'iframe' ||
                    getPDFurl(getItemSrc(itemIndex))
                "
                :key="itemIndex"
                frameborder="0"
                allowfullscreen
              >
              </iframe>
              <video
                v-autoplayObserver
                :data-autoplay="setAutoplay(itemIndex)"
                class="cool-lightbox-video"
                v-if="
                  checkIsMp4(getItemSrc(itemIndex)) ||
                    getMediaType(itemIndex) === 'webVideo'
                "
                :style="aspectRatioVideo"
                :key="checkIsMp4(getItemSrc(itemIndex))"
                controls=""
                controlslist="nodownload"
                l
                poster=""
              >
                <source
                  :src="checkIsMp4(getItemSrc(itemIndex))"
                  :type="
                    'video/' +
                      (getVideoExt(getItemSrc(itemIndex))
                        ? getVideoExt(getItemSrc(itemIndex))
                        : getExtFromItem(itemIndex))
                  "
                />
                Sorry, your browser doesn't support embedded videos
              </video>
            </div>
          </div>
          <!--/cool-lightbox__slide-->
        </div>
        <!--/cool-lightbox-wrapper-->
        <div v-if="effect === 'fade'" class="cool-lightbox__wrapper">
          <div
            ref="itemsRef"
            class="cool-lightbox__slide cool-lightbox__slide--current"
          >
            <transition name="cool-lightbox-slide-change" mode="out-in">
              <div
                v-if="getMediaType(imgIndex) === 'image'"
                key="image"
                :style="imgWrapperStyle"
                class="cool-lightbox__slide__img"
              >
                <transition
                  name="cool-lightbox-slide-change"
                  mode="out-in"
                  v-if="!isItemPicture(imgIndex)"
                >
                  <img
                    :src="getItemSrc(imgIndex)"
                    :srcset="getItemSrcSet(imgIndex)"
                    :sizes="getItemSizes(imgIndex)"
                    :key="imgIndex"
                    draggable="false"
                    :alt="getItemAlt(imgIndex)"
                    @load="imageLoaded"
                    @click="zoomImage"
                    @mousedown="handleMouseDown($event)"
                    @mouseup="handleMouseUp($event)"
                    @mousemove="handleMouseMove($event)"
                  />
                </transition>
                <transition
                  v-else
                  name="cool-lightbox-slide-change"
                  mode="out-in"
                >
                  <picture :key="imgIndex">
                    <source
                      v-for="(source, sourceIndex) in getPictureSources(
                        imgIndex
                      )"
                      :srcset="source.srcset"
                      :type="source.type"
                      :media="source.media"
                      :sizes="source.sizes || getItemSizes(imgIndex)"
                      :key="`source-${imgIndex}-${sourceIndex}`"
                    />
                    <img
                      :src="getItemSrc(imgIndex)"
                      :srcset="getItemSrcSet(imgIndex)"
                      :sizes="getItemSizes(imgIndex)"
                      draggable="false"
                      :alt="getItemAlt(imgIndex)"
                      @load="imageLoaded"
                      @click="zoomImage(imgIndex)"
                      @mousedown="handleMouseDown($event)"
                      @mouseup="handleMouseUp($event)"
                      @mousemove="handleMouseMove($event)"
                    />
                  </picture>
                </transition>

                <div
                  v-show="imageLoading"
                  class="cool-lightbox-loading-wrapper"
                >
                  <slot name="loading">
                    <div class="cool-lightbox-loading"></div>
                  </slot>
                </div>
                <!--/loading-wrapper-->
              </div>
              <!--/imgs-slide-->
              <div v-else key="video" class="cool-lightbox__iframe">
                <transition name="cool-lightbox-slide-change" mode="out-in">
                  <div>
                    <iframe
                      class="cool-lightbox-video"
                      v-autoplayObserver
                      :data-autoplay="setAutoplay(imgIndex)"
                      :src="getVideoUrl(getItemSrc(imgIndex))"
                      v-if="
                        !checkIsMp4(getItemSrc(imgIndex)) &&
                          getMediaType(imgIndex) === 'video'
                      "
                      :style="aspectRatioVideo"
                      :key="getVideoUrl(getItemSrc(imgIndex))"
                      frameborder="0"
                      allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                      allowfullscreen
                    >
                    </iframe>
                    <iframe
                      class="cool-lightbox-pdf"
                      :src="getItemSrc(imgIndex)"
                      v-if="
                        getMediaType(imgIndex) === 'iframe' ||
                          getPDFurl(getItemSrc(imgIndex))
                      "
                      :key="imgIndex"
                      frameborder="0"
                      allowfullscreen
                    >
                    </iframe>
                    <video
                      class="cool-lightbox-video"
                      v-autoplayObserver
                      :data-autoplay="setAutoplay(imgIndex)"
                      v-if="
                        checkIsMp4(getItemSrc(imgIndex)) ||
                          getMediaType(imgIndex) === 'webVideo'
                      "
                      :style="aspectRatioVideo"
                      :key="checkIsMp4(getItemSrc(imgIndex))"
                      controls=""
                      controlslist="nodownload"
                      poster=""
                    >
                      <source
                        :src="checkIsMp4(getItemSrc(imgIndex))"
                        :type="
                          'video/' +
                            (getVideoExt(getItemSrc(imgIndex))
                              ? getVideoExt(getItemSrc(imgIndex))
                              : getExtFromItem(imgIndex))
                        "
                      />
                      Sorry, your browser doesn't support embedded videos
                    </video>
                  </div>
                </transition>
              </div>
              <!--/cool-lightbox__iframe-->
            </transition>
          </div>
          <!--/cool-lightbox__slide-->
        </div>
        <!--/cool-lightbox__wrapper-->
        <transition name="cool-lightbox-modal">
          <div
            v-show="
              checkIfIsObject(imgIndex) &&
                (items[imgIndex].title || items[imgIndex].description)
            "
            key="caption-block"
            class="cool-lightbox-caption"
          >
            <transition name="cool-lightbox-slide-change" mode="out-in">
              <h6
                key="title"
                v-html="items[imgIndex].title"
                v-if="checkIfIsObject(imgIndex) && items[imgIndex].title"
              ></h6>
            </transition>

            <transition name="cool-lightbox-slide-change" mode="out-in">
              <p
                key="description"
                v-html="items[imgIndex].description"
                v-if="checkIfIsObject(imgIndex) && items[imgIndex].description"
              ></p>
            </transition>
          </div>
          <!--/cool-lightbox-caption-->
        </transition>

        <div class="cool-lightbox-toolbar" :class="buttonsClasses">
          <button
            type="button"
            v-if="this.slideshow && items.length > 1"
            title="Play slideshow"
            class="cool-lightbox-toolbar__btn"
            @click="togglePlaySlideshow"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              v-if="!isPlayingSlideShow"
              viewBox="0 0 24 24"
            >
              <path d="M6.5 5.4v13.2l11-6.6z"></path>
            </svg>

            <svg v-else xmlns="http://www.w3.org/2000/svg">
              <g>
                <rect
                  id="svg_4"
                  height="11.97529"
                  width="11.728392"
                  y="6.030873"
                  x="6.259265"
                  stroke-width="1.5"
                  stroke="#000"
                  fill="#000000"
                />
              </g>
            </svg>
          </button>

          <button
            type="button"
            @click="showThumbs = !showThumbs"
            title="Show thumbnails"
            v-if="items.length > 1 && gallery"
            class="cool-lightbox-toolbar__btn"
          >
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
              <path
                d="M14.59 14.59h3.76v3.76h-3.76v-3.76zm-4.47 
              0h3.76v3.76h-3.76v-3.76zm-4.47 0h3.76v3.76H5.65v-3.76zm8.94-4.47h3.76v3.76h-3.76v-3.76zm-4.47 
              0h3.76v3.76h-3.76v-3.76zm-4.47 0h3.76v3.76H5.65v-3.76zm8.94-4.47h3.76v3.76h-3.76V5.65zm-4.47 
              0h3.76v3.76h-3.76V5.65zm-4.47 0h3.76v3.76H5.65V5.65z"
              ></path>
            </svg>
          </button>

          <button
            type="button"
            v-if="fullScreen"
            @click="toggleFullScreenMode"
            class="cool-lightbox-toolbar__btn"
            title="Fullscreen"
          >
            <svg
              width="20px"
              height="20px"
              viewBox="0 0 18 18"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M4.5 11H3v4h4v-1.5H4.5V11zM3 7h1.5V4.5H7V3H3v4zm10.5 6.5H11V15h4v-4h-1.5v2.5zM11 3v1.5h2.5V7H15V3h-4z"
              ></path>
            </svg>
          </button>

          <button
            type="button"
            v-if="showCloseButton"
            class="cool-lightbox-toolbar__btn"
            title="Close"
            @click="close"
          >
            <slot name="close">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                <path
                  d="M12 10.6L6.6 5.2 5.2 6.6l5.4 5.4-5.4 5.4 1.4 1.4 5.4-5.4 5.4 5.4 1.4-1.4-5.4-5.4 5.4-5.4-1.4-1.4-5.4 5.4z"
                ></path>
              </svg>
            </slot>
          </button>
        </div>
        <!--/cool-lightbox--toolbar-->
      </div>

      <transition name="cool-lightbox-modal">
        <div v-if="isZooming && useZoomBar" class="cool-lightbox-zoom">
          <svg
            height="469pt"
            class="cool-lightbox-zoom__icon"
            viewBox="0 -192 469.33333 469"
            width="469pt"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="m437.332031.167969h-405.332031c-17.664062 
            0-32 14.335937-32 32v21.332031c0 17.664062 14.335938 32 32 32h405.332031c17.664063 0 32-14.335938 
            32-32v-21.332031c0-17.664063-14.335937-32-32-32zm0 0"
            />
          </svg>
          <input
            type="range"
            v-model="zoomBar"
            name="points"
            min="0"
            max="50"
          />
          <svg
            height="426.66667pt"
            class="cool-lightbox-zoom__icon"
            viewBox="0 0 426.66667 426.66667"
            width="426.66667pt"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="m405.332031 192h-170.664062v-170.667969c0-11.773437-9.558594-21.332031-21.335938-21.332031-11.773437 0-21.332031 
            9.558594-21.332031 21.332031v170.667969h-170.667969c-11.773437 0-21.332031 9.558594-21.332031 21.332031 0 
            11.777344 9.558594 21.335938 21.332031 21.335938h170.667969v170.664062c0 11.777344 9.558594 21.335938 21.332031 
            21.335938 11.777344 0 21.335938-9.558594 21.335938-21.335938v-170.664062h170.664062c11.777344 0 21.335938-9.558594 
            21.335938-21.335938 0-11.773437-9.558594-21.332031-21.335938-21.332031zm0 0"
            />
          </svg>
        </div>
      </transition>
    </div>
  </transition>
</template>
<script src="./TD_lightBox.js"></script>
