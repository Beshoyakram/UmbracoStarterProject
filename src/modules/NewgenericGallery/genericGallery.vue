
<template>
    <div class="generic-gallery-wrapper about-ministry">
        <!-- full screen starts -->
        <gallery-pop-up @closePopup="isPopupOpened = !isPopupOpened"
                        v-if="isPopupOpened"
                        :data="apiData.mediaItems"
                        :initialIndex="fullscreenItemIndex">
        </gallery-pop-up>
        <!-- full screen ends -->
        <section class="container media-gallery generic-gallery generic-gallery--no-padding"
                 v-if="
                 (!isLoading && apiSucess && apiData.mediaItems.length>
            0) || isLoading
            "
            >
            <div class="wrapper position-relative">
                <h1 class="title generic-gallery__title text-center"
                    v-html="props.title"></h1>
                <!-- main slider starts -->
                <swiper v-bind="swiperOptionsModel"
                        :modules="swiperModules.modules"
                        v-on="mainSliderEvents"
                        v-if="apiData != null"
                        class="generic-gallery__slides-wrapper"
                        :key="'genericGallary' + swiperKey">
                    <swiper-slide class="generic-gallery__slide"
                                  v-for="(item, index) in apiData.mediaItems"
                                  :key="index"
                                  :data-item-index="index">
                        <div class="aspect-ratio pointer">
                            <div @click.prevent="openFullScreenGallery(index)"
                                 class="generic-gallery__slide__img aspect-ratio__inner">
                                <img class="aspect-ratio__inner__img" :src="item.thumbImage" />
                                <span class="icon-video" v-if="item.mediaType != 'image'">
                                    <span class="path1"></span>
                                    <span class="path2"></span>
                                </span>
                                <span class="brief text-center"
                                      :title="item.mediaDescription">{{ item.mediaDescription }}</span>
                            </div>
                        </div>
                    </swiper-slide>
                </swiper>
                <!-- main slider ends -->
                <!-- navigation starts -->
                <div v-if="isNeedNavs" class="generic-gallery__navigation">
                    <span :class="`icon-right-arrow-long-${this.$attrs.props.number}`" class="prev icon-right-arrow-long"></span>
                    <span :class="`icon-left-arrow-long-${this.$attrs.props.number}`" class="next icon-left-arrow-long"></span>
                </div>
                <!-- navigation ends -->
            </div>
            <!-- Start Loader -->
            <Loader :show="isLoading" />
            <!-- End Loader -->
        </section>
    </div>
</template>

<script>
    import { Swiper, SwiperSlide } from "swiper/vue/swiper-vue";
    import GalleryPopUp from "./components/popup/GalleryPopUp.vue";
    import { callGenericGallery } from "./services.js";
    import { FreeMode, Navigation, Autoplay, Pagination, Scrollbar } from "swiper";
    import "swiper/swiper-bundle.css";
    import { nextTick } from "@vue/runtime-core";
    import Loader from "../global/components/loader/Loader.vue";

    let instance;
    export default {
        name: "GenericGallery",
        components: {
            Swiper,
            SwiperSlide,
            GalleryPopUp,
            Loader,
        },
        data() {
            return {
                swiperKey: 1,
                props: {},
                desktop: undefined,
                mobile: undefined,
                tablet: undefined,
                isPopupOpened: false,
                pageDirection: "rtl",
                activeThumbBackgroundUrl: "",
                apiData: null,
                fullScreenItems: [],
                fullscreenItemIndex: null,
                currentActiveItemInThumb: 1,
                isLoading: true,
                apiSucess: true,
                swiperParams: {
                    loop: false,
                    center: true,
                    simulateTouch: false,
                    spaceBetween: 15,
                    slidesPerView: 1.15,
                    breakpoints: {
                        768: {
                            spaceBetween: 20,
                            slidesPerView: 4,
                        },
                    },
                }, // for more params go to https://swiperjs.com/swiper-api#parameters
                swiperModules: {
                    modules: [Pagination, Navigation, Autoplay, Scrollbar, FreeMode],
                    modulesProps: {
                        navigation: {
                            nextEl: `.media-gallery .generic-gallery__navigation .icon-left-arrow-long-${this.$attrs.props.number}`,
                            prevEl: `.media-gallery .generic-gallery__navigation .icon-right-arrow-long-${this.$attrs.props.number}`,
                        },
                        freemode: true, // for custom props go to https://swiperjs.com/swiper-api#free-mode
                    },
                }, // for more modules go to https://swiperjs.com/swiper-api#modules
                mainSliderEvents: {
                    realIndexChange: this.handleActiveChange,
                    swiper: this.initSwiper,
                },
            };
        },
        mounted() {
            this.initdesktopMediaQueryListener();
            instance = this;
            this.props = this.$attrs.props;
            if (this.props.lang != "ar-EG") {
                this.pageDirection = "ltr";
            }
            //callGenericGallery(
            //    `./JsonFiles/GallerySample.json`,
            callGenericGallery(
                `umbraco/api/Gallery/GetItemsInGallery?pageId=${this.props.pageId}&lang=${this.props.lang}`,
                (res) => {
                    this.apiData = res.data;
                    // this.apiData.mediaItems = [
                    //     this.apiData.mediaItems[0],
                    // this.apiData.mediaItems[0],
                    //    ...this.apiData.mediaItems ,
                    //    ...this.apiData.mediaItems ,
                    // ]
                    if (this.apiData.mediaItems.length > 0) {
                        for (let i = 0; i < this.apiData.mediaItems.length; i++) {
                            if (this.apiData.mediaItems[i].mediaType == "youtube") {
                                let youtubeLinkId =
                                    this.apiData.mediaItems[i].mediaUrl.split("embed/")[1];
                                this.apiData.mediaItems[
                                    i
                                ].thumbImage = `https://img.youtube.com/vi/${youtubeLinkId}/hqdefault.jpg`;
                            }
                            if (i == 0) {
                                this.activeThumbBackgroundUrl =
                                    this.apiData.mediaItems[i].thumbImage;
                            }
                            instance.fullScreenItems.push({
                                title: this.apiData.mediaItems[i].mediaDescription,
                                thumb: this.apiData.mediaItems[i].thumbImage,
                                src: this.apiData.mediaItems[i].mediaUrl,
                            });
                        }
                        instance.centerThumbItems();

                        if (this.apiData.mediaItems.length > 4) {
                            this.swiperParams.loop = true;
                            this.isNeedNavs = true;
                            this.swiperKey++;
                        }
                    }

                    this.isLoading = false;
                    this.apiSucess = true;
                    setTimeout(() => {
                        /* eslint-disable no-debugger */
                        //debugger;
                        if (this.apiData.mediaItems.length > 1) {
                            if (
                                document.querySelectorAll(".generic-gallery-slider .thumbs__wrapper")[parseInt(this.props.number) - 1]
                            ) {
                                document
                                    .querySelectorAll(".generic-gallery-slider .thumbs__wrapper")[parseInt(this.props.number) - 1]
                                    .classList.add("swiper-ready");
                            }
                        }
                    }, 0);
                },
                // eslint-disable-next-line no-unused-vars
                (error) => {
                    document.querySelector("#snackbar").innerText = this.props.apiError;
                    document.querySelector("#snackbar").classList.add("show");
                    setTimeout(function () {
                        document.querySelector("#snackbar").classList.remove("show");
                    }, 3000);
                    this.isLoading = false;
                    this.apiSucess = false;
                }
            );

            //setting class for parent element , to center items in case they are less than 6 in desktop , 2 in mobile
            // window.addEventListener('resize', function () {
            //     instance.centerThumbItems();
            // });
        },
        methods: {
            centerThumbItems() {
                //setting class for parent element , to center items in case they are less than 6 in desktop , 2 in mobile
                if (!this.isNeedNavs) {
                    document
                        .querySelectorAll(".media-gallery.generic-gallery")[parseInt(this.props.number) - 1]
                        .classList.add("generic-gallery--center");
                } else {
                    if (
                        document
                            .querySelectorAll(".media-gallery.generic-gallery")[parseInt(this.props.number) - 1]
                            .classList.contains("generic-gallery--center")
                    ) {
                        document
                            .querySelectorAll(".generic-gallery")[parseInt(this.props.number) - 1]
                            .classList.remove("generic-gallery--center");
                    }
                }
                if (this.apiData?.mediaItems?.length == 1) {
                    document
                        .querySelectorAll(".media-gallery.generic-gallery")[parseInt(this.props.number) - 1]
                        .classList.add("generic-gallery--one");
                } else {
                    if (
                        document
                            .querySelectorAll(".media-gallery.generic-gallery")[parseInt(this.props.number) - 1]
                            .classList.contains("generic-gallery--one")
                    ) {
                        document
                            .querySelectorAll(".media-gallery.generic-gallery")[parseInt(this.props.number) - 1]
                            .classList.remove("generic-gallery--one");
                    }
                }
            },
            openFullScreenGallery(index) {
                //TODO remaining fullscreen mode
                this.fullscreenItemIndex = index;
                this.isPopupOpened = true;
            },
            handleActiveChange() {
                // this.swiper.slideTo(3)
                // this.currentActiveItemInThumb = swiper.activeIndex + 1;
                // this.activeThumbBackgroundUrl = this.apiData.mediaItems[swiper.activeIndex].thumbImage;
                // //this means that our prev slider was a video
                // if (document.querySelectorAll(".generic-gallery-slider .gallery-top__item")[swiper.previousIndex].querySelector("img") == null) {
                //     if (document.querySelectorAll(".generic-gallery-slider .gallery-top__item")[swiper.previousIndex].querySelector("video") != null) {
                //         //reset video so it won't continue playing in the background
                //         document.querySelectorAll(".generic-gallery-slider .gallery-top__item")[swiper.previousIndex].querySelector("video").currentTime = 0;
                //         document.querySelectorAll(".generic-gallery-slider .gallery-top__item")[swiper.previousIndex].querySelector("video").pause();
                //     } else {
                //         //reset iframe so it won't continue playing in the background
                //         document.querySelectorAll(".generic-gallery-slider .gallery-top__item")[swiper.previousIndex].querySelector("iframe").src = document.querySelectorAll(".generic-gallery-slider .gallery-top__item")[swiper.previousIndex].querySelector("iframe").src;
                //     }
                // }
            },
            initSwiper(swiper) {
                this.swiper = swiper;
                // this.swiper.slideTo(3)
                //alert("initSwiper");
            },
            initdesktopMediaQueryListener() {
                const desktopmediaQueryList = window.matchMedia("(max-width: 1199px)");
                const mobilemediaQueryList = window.matchMedia("(max-width: 767px)");
                this.detectCurrentDevice(desktopmediaQueryList, mobilemediaQueryList);
                if (this.checkBrowserCompatability(desktopmediaQueryList))
                    desktopmediaQueryList.addEventListener("change", (e) => {
                        this.desktopMediaQueryHandler(e, mobilemediaQueryList);
                    });
                else
                    desktopmediaQueryList.addListener((e) => {
                        this.desktopMediaQueryHandler(e, mobilemediaQueryList);
                    });

                if (this.checkBrowserCompatability(desktopmediaQueryList))
                    mobilemediaQueryList.addEventListener("change", (e) => {
                        this.mobileMediaQueryHandler(e);
                    });
                else
                    mobilemediaQueryList.addListener((e) => {
                        this.mobileMediaQueryHandler(e);
                    });
            },
            checkBrowserCompatability(mediaQueryList) {
                if (mediaQueryList.addEventListener == undefined) {
                    return false;
                }
                return true;
            },
            detectCurrentDevice(desktopmediaQueryList, mobilemediaQueryList) {
                if (desktopmediaQueryList.matches) {
                    if (mobilemediaQueryList.matches) {
                        this.mobile = true;
                        this.tablet = false;
                    } else {
                        this.tablet = true;
                        this.mobile = false;
                    }
                } else this.desktop = true;
            },
            mobileMediaQueryHandler(e) {
                if (e.matches) {
                    this.mobile = true;
                    this.tablet = false;
                } else {
                    this.mobile = false;
                    this.tablet = true;
                }
            },
            desktopMediaQueryHandler(e, mobilemediaQueryList) {
                if (e.matches) {
                    if (mobilemediaQueryList.matches) {
                        this.mobile = true;
                        this.tablet = false;
                    } else {
                        this.tablet = true;
                        this.mobile = false;
                    }
                } else {
                    this.desktop = true;
                }
            },
        },
        computed: {
            swiperOptionsModel() {
                let modulesProps;
                if (!this.swiperModules.modulesProps.thumbs)
                    modulesProps = Object.assign(this.swiperModules.modulesProps, {
                        thumbs: { swiper: null },
                    });
                else modulesProps = this.swiperModules.modulesProps;
                let model = { ...modulesProps, ...this.swiperParams };
                return model;
            },
            isNeedNavs() {
                if (this.mobile) {
                    return (
                        this.apiData?.mediaItems?.length >
                        Math.floor(this.swiperParams.slidesPerView)
                    );
                }
                if (this.tablet) {
                    return (
                        this.apiData?.mediaItems?.length >
                        this.swiperParams.breakpoints[768].slidesPerView
                    );
                }
                return (
                    this.apiData?.mediaItems?.length >
                    this.swiperParams.breakpoints[768].slidesPerView
                );
            },
        },
        watch: {
            desktop() {
                if (this.desktop) {
                    nextTick(() => {
                        this.centerThumbItems();
                    });
                }
            },
            tablet() {
                if (this.tablet) {
                    nextTick(() => {
                        this.centerThumbItems();
                    });
                }
            },
            mobile() {
                if (this.mobile) {
                    nextTick(() => {
                        this.centerThumbItems();
                    });
                }
            },
            isNeedNavs() {
                nextTick(() => {
                    this.centerThumbItems();
                });
            },
        },
    };
</script>
