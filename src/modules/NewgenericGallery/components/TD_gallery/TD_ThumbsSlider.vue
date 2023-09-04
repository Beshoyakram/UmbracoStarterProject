<template>

<slider ref="swiperThumbContainer" v-if="images.length > 1"  
:images="images"
:events="eventModel" 
:swiperParams="swiperParams" 
:swiperModules="swiperModules"
>
    <template v-slot:slide="{ slideModel , index }" >
               <slot name="slide" :slideModel="slideModel" :index="index" ></slot>
    </template>
</slider>
</template>
<script src="./TD_thumbsSlider.js" ></script>
