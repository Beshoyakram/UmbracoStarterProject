<template>
  <swiper ref="swiperContainer"
    v-bind="swiperOptionsModel"
    :modules="swiperModules.modules"
    v-on="events"
  >
  <!-- default swiper slots starts -->
  <template v-slot:container-start>
      <div>
        <slot name="container-start"></slot>
      </div>
  </template>
    <template v-slot:container-end>
      <div>
        <slot name="container-end"></slot>
      </div>
  </template>
    <template v-slot:wrapper-start>
      <div>
        <slot name="wrapper-start"></slot>
      </div>
  </template>
    <template v-slot:wrapper-end>
      <div>
        <slot name="wrapper-end"></slot>
      </div>
  </template>
  <!-- default swiper slots ends -->
  <!-- slides starts -->
    <swiper-slide
      v-for="(image, index) in images"
      :key="index"
    >
        <slot name="slide" :slideModel="image" :index="index" ></slot>
    </swiper-slide>
  <!-- slides ends -->
  </swiper>
</template>
<script src="./TD_slider.js" ></script>


