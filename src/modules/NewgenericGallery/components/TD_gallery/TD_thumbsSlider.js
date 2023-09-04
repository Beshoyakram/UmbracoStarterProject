import Slider from "./TD_Slider.vue"
import swiperBaseMixin from "./TD_galary.mixin";
import "swiper/modules/thumbs/thumbs.min.css";
import {ref ,nextTick } from "vue";
export default {
  name: "TD_ThumbsSlider",
  mixins: [ swiperBaseMixin ],
  components: {
    Slider,
  },
  data() {
    return {
      isThumbsCentered : false ,
      thumbsCheckIsCenteredQueryList : [
        320 ,
        768 ,
        1024 ,
        1366 ,
        1920
      ] ,
      thumbEvents : {
        swiper : this.onThumbSwiperMounted ,
        breakpoint : this.onBreakPoint
      }
    }
  },
  setup() {
    const swiperThumbContainer = ref(null);
    return { swiperThumbContainer }
  },
  computed : {
      eventModel(){
          return { ...this.events ,  ...this.thumbEvents }
      } ,
  } ,
  methods: {
        onThumbSwiperMounted (swiper) {
          nextTick(()=>{
            this.swiperParams.loop || this.performThumbsCenter(swiper.width , this.getSlidesTotalWidth(swiper.slidesGrid[1] , swiper.slidesSizesGrid.length ) )
          }) 
        this.$emit('thumbsMounted', swiper)
        } ,
        onBreakPoint(swiper){
          nextTick(()=>{
            this.swiperParams.loop || this.performThumbsCenter(swiper.width , this.getSlidesTotalWidth(swiper.slidesGrid[1] , swiper.slidesSizesGrid.length ) )
          }) 
        },
        getSlidesTotalWidth(slideWidth , sildesNumber ){
          return slideWidth * sildesNumber
        },
        performThumbsCenter( parentWidth , childrenWidth){
          this.checkIsThumbsNeedToCenter(parentWidth , childrenWidth) ? this.centerThumbs() : this.cancelThumbsCenter()
        },
        checkIsThumbsNeedToCenter(parentWidth , childrenWidth){
          return parentWidth > childrenWidth
        },
        centerThumbs(){
          this.swiperThumbContainer.$el.querySelector(".swiper-wrapper").classList.add("swiper-wrapper--center")
        } ,
        cancelThumbsCenter(){
          this.swiperThumbContainer.$el.querySelector(".swiper-wrapper").classList.remove("swiper-wrapper--center")
        }
  },

};