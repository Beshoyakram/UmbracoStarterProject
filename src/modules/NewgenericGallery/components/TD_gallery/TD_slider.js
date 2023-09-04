import { Swiper, SwiperSlide } from "swiper/vue/swiper-vue";
import swiperBaseMixin  from "./TD_galary.mixin";
//#region styles
// Import Swiper styles
import "swiper/swiper-bundle.css";
//#endregion
export default {
  name: "TD_Slider",
  mixins: [swiperBaseMixin ],
  components: {
    Swiper,
    SwiperSlide,
  },
  mounted() {
  },
};