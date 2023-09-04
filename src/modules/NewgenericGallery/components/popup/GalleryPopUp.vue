<template>
  <div class="td-popup">
    <div class="td-popup__background" @click="close" ></div>
    <div class=" td-popup__content-wrapper ">
      <div class="container">
        <div class=" slides__control">
          <div @click="close" class="close ">
            <span class="icon-close-icon"></span>
          </div>

        </div>

        <div class="td-popup__content">
            <span
              class="image-wrapper__bg"
              :style="{
                backgroundImage: `url(${data[currentIndex].thumbImage})`,
              }"
            ></span>
          <div class="td-generic-slider">

            <slider
              key="main-slider"
              class="td-generic-slider__main-slider"
              :swiperModules="swiperModules"
              :swiperParams="swiperParams"
              :events="events"
              :images="data"
              :bindSwiperWithThumbs="true"
              :thumbsSwiper="thumbs"
            >
              <template v-slot:slide="{ slideModel }">
                <div class="popup-slide">

                  <!-- <span class="full-screen">
                                                <span class="icon-expand-out"></span>
                                            </span> -->
                  <div class=" popup-slide__image-wrapper">

                    <div
                      class="popup-slide__image-wrapper__main-image aspect-ratio"
                    >
                      <div class="aspect-ratio__inner">
                                <img class="aspect-ratio__inner__img w-100" :src="slideModel.mediaUrl" v-if="slideModel.mediaType=='image'" />
                                <video class="aspect-ratio__inner__img w-100 h-100" controls="controls"
                                 v-if="slideModel.mediaType=='internalVideo'"
                                 @playing="handlePlay"
                                 @pause="pauseVideo"
                                 >
                                    <source :src="slideModel.mediaUrl" type="video/mp4" />
                                    <source :src="slideModel.mediaUrl" type="video/webm" />
                                    <source :src="slideModel.mediaUrl" type="video/ogg" />
                                </video>
                                <iframe
                                    @load="addYouTubePlayListener"
                                 class="aspect-ratio__inner__img w-100 h-100" 
                                 :src="slideModel.mediaUrl" v-if="slideModel.mediaType=='youtube'"></iframe>
                      </div>
                    </div>
                  </div>
                  <!-- <img :src="slideModel.mediaUrl" :alt="slideModel.mediaUrl" /> -->
                </div>
              </template>
            </slider>
            <div v-if="data.length > 1" class="thumbs-slider-wrapper">
              <div class="row justify-content-center">
                <div class="col-12 col-md-8 p-0 thumbs-slider-wrapper__layout">
                  <span v-if="isNeedNavs" @click="swiper.slidePrev" class="icon-right-arrow-long prev"
                  :class="[isFirstIndex ? 'nav-disabled' : ''] "
                  
                    ></span>
                  <div class="thumb-col">
                      <thumb-slider
                        key="thumbs-slider"
                        class="generic-slider__thumbs-slider"
                        :swiperModules="thumbsConfig.swiperModules"
                        :swiperParams="thumbsConfig.swiperParams"
                        :images="data"
                        @thumbsMounted="setMainSliderThumb"
                        :events="thumbsEvents"
                      >
                        <template v-slot:slide="{ slideModel }">
                          <div class="thumbs-slide pointer">
                            <div class="aspect-ratio">
                                <div class="aspect-ratio__inner ">
                                    <img class="aspect-ratio__inner__img thumbs-slide__img " :src="slideModel.thumbImage" />
                                </div>

                            </div>
                          </div>
                        </template>
                      </thumb-slider>
                  </div>
                  <span v-if="isNeedNavs" @click="swiper.slideNext" 
                  :class="[isLastIndex ? 'nav-disabled' : '' ]"
                  class="icon-left-arrow-long next" ></span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import Slider from "../TD_gallery/TD_Slider.vue";
import ThumbSlider from "../TD_gallery/TD_ThumbsSlider.vue";

import {  FreeMode, Thumbs , Keyboard } from "swiper";
import "swiper/modules/free-mode/free-mode.min.css";
import "swiper/modules/navigation/navigation.min.css";
import { nextTick } from '@vue/runtime-core';
// import "../components/TD_gallery/TD_gallery.scss"
export default {
  name: "GalleryPopUp",
  components: {
    Slider,
    ThumbSlider,
  },
  props: {
    data: {
      type: Object,
      required: true,
    },
    initialIndex : {
      type : Number ,
      default : 0 ,
    }
  },
  data() {
    return {
      desktop:undefined,
      mobile : undefined , 
      tablet : undefined ,
      isFirstIndex:undefined ,
      isLastIndex:undefined ,
      thumbs: null,
      swiper : null , 
      currentPage: this.initialIndex + 1,
      currentIndex : this.initialIndex ,
      playingVideoElement : undefined ,
      isVideoPlaying : false , 
      swiperParams: {
        allowTouchMove: false,
        initialSlide : this.initialIndex
      }, // for more params go to https://swiperjs.com/swiper-api#parameters
      events: {
        slideChange: (swiper) => {
          if(this.data[this.currentIndex].mediaType=='youtube')
              this.resetCurrentIframe()
          this.currentPage = swiper.activeIndex + 1;
          this.currentIndex = swiper.activeIndex;
          this.pausePlayingVideos()
        },
        reachEnd: () => {
          this.$emit("reachEnd");
        },
         swiper :(swiper)=>  this.initSwiper(swiper)
      },
      swiperModules: {
        modules: [FreeMode, Thumbs , Keyboard],
        modulesProps: {
          keyboard:true ,
        },
      },
      thumbsConfig: {
        swiperParams: {
          watchSlidesProgress: true,
          slidesPerView: 3, // default in no breakpoints
          spaceBetween: 15, // default in no breakpoints
          breakpoints: {
            425: {
              slidesPerView: 4,
              spaceBetween: 15,
            },
            1025: {
              slidesPerView: 6,
              spaceBetween: 15,
            },
          }, // for less than 320 default will be applied
        }, // for more params go to https://swiperjs.com/swiper-api#parameters
        swiperModules: {
          modules: [FreeMode],
          modulesProps: {
            // navigation: {
            //   nextEl : ".swiper-button-next" ,
            //   prevEl : ".swiper-button-prev" ,
            // } ,  // for custom props go to https://swiperjs.com/swiper-api#navigation
          },
        }, // for more modules go to https://swiperjs.com/swiper-api#modules

      },
      thumbsEvents : {
        slideChange: () => {
          this.thumbs.slides.forEach( (slide , index ) => {
            if(index == this.swiper.activeIndex){
              slide.classList.add('swiper-slide-thumb-active')
            }
            else {
              if(slide.classList.contains('swiper-slide-thumb-active'))
                slide.classList.remove('swiper-slide-thumb-active')
            }
          });
        } ,
      }
    };
  },
  methods: {
    close() {
      this.$emit("closePopup", this.currentPage - 1);
    },
    setMainSliderThumb(swiper){
      this.thumbs = swiper;
    } ,
    initSwiper(swiper){
      this.swiper = swiper
    } ,
    addYouTubePlayListener(/*e*/){
      // console.log( e.path[0])
      // e.path[0].addEventListener("onStateChange" , ()=>{
      //   console.log("playing")
      // })
    },
    pausePlayingVideos(){
        if(this.isVideoPlaying)
          this.playingVideoElement.pause();
    } ,
    pauseVideo(){
    this.isVideoPlaying = false;
    this.playingVideoElement = undefined;
    } ,
    playVideo(video){
      this.isVideoPlaying = true;
      this.playingVideoElement = video;
    },
    handlePlay(e){
      this.pausePlayingVideos();
      this.playVideo(e.target)
    } ,
    resetCurrentIframe() {
      let iframe =  document.querySelectorAll(".td-generic-slider__main-slider .swiper-wrapper .swiper-slide")[this.currentIndex].querySelector("iframe")
       let src = iframe.src 
        iframe.src = src
    },
    initdesktopMediaQueryListener(){
            const desktopmediaQueryList = window.matchMedia('(max-width:1024px)');
            const mobilemediaQueryList = window.matchMedia('(max-width: 424px)');
            if(desktopmediaQueryList.matches){
                if(mobilemediaQueryList.matches){
                    this.mobile = true;
                    this.tablet  = false;
                } else {
                    this.tablet = true;
                    this.mobile = false;
                }
            }else {
                this.desktop = true;
            }
            desktopmediaQueryList.addEventListener("change" , (e)=>{
                this.desktop  = e.matches ? false : true;
            })

            mobilemediaQueryList.addEventListener("change" , (e)=>{
              if( e.matches){
                  this.mobile  = true;
                }
                else {
                  this.tablet = true;
                  this.mobile  = false;
                }
            })
    } ,
    centerThumbItems() {
                //setting class for parent element , to center items in case they are less than 6 in desktop , 2 in mobile
                if ( !this.isNeedNavs ) {
                        document.querySelector(".thumb-col").classList.add('fill-100');
                } else {
                        document.querySelector(".thumb-col").classList.remove('fill-100');
                }
    },
    onIndexChange(){
          if(this.currentIndex == 0){
            this.isFirstIndex = true 
          }else{
            this.isFirstIndex = false 
          }
          if(this.currentIndex == this.data.length - 1){
            this.isLastIndex = true;
          }else{
            this.isLastIndex = false 
          }
    }
  },
  computed : {
    isNeedNavs(){
        if(this.mobile){
          return  this.data?.length > Math.floor(this.thumbsConfig.swiperParams.slidesPerView)
        }
        if(this.tablet){
            return this.data?.length > Math.floor(this.thumbsConfig.swiperParams.breakpoints[425].slidesPerView)
       }
        return   this.data?.length > this.thumbsConfig.swiperParams.breakpoints[1025].slidesPerView
    }
  },
    watch : {
        currentIndex(){
            this.onIndexChange();
        },
        desktop(){
            if(this.desktop){
                this.tablet = false;
                this.mobile = false;
                nextTick(()=>{
                    this.centerThumbItems();
                })
            }else {
                if(!this.mobile){
                    this.tablet = true;
                nextTick(()=>{
                    this.centerThumbItems();
                })
                }
            }
        },
        mobile(){
            if(this.mobile){
                this.tablet = false;
                this.desktop = false;
                nextTick(()=>{
                    this.centerThumbItems();
                })
            }else {
              if(!this.desktop){
                this.tablet = true;
                nextTick(()=>{
                    this.centerThumbItems();
                })
                } 
            }
        } ,
    },
  mounted() {
    document.body.style.overflow = "hidden";
    this.initdesktopMediaQueryListener();
    this.onIndexChange()
  },
  beforeUnmount() {
    document.body.style.overflow = "auto";
  },
};
</script>
