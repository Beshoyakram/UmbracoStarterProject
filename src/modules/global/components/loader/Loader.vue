<template>
    <div class="loader" v-show="show">
        <div class="lds-spinner">
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
        </div>
    </div>
</template>

<script>
export default {
    name : "loader" ,
    props : {
        show : {
            type : Boolean ,
            required : true ,
        }
    }
}
</script>