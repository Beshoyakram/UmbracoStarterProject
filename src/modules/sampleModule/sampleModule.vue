<template>
    <p>Sample Module Loaded Successfully</p>
    <p>{{props.siteTitle}} --loaded from vue component</p>
</template>

<script>
    export default {
        // eslint-disable-next-line
        components: {},
        data() {
            return {
                props: {},
            };
        },
        mounted() {
            /* eslint-disable no-debugger */
            //debugger;
            //this.$attrs.props

            this.props = this.$attrs.props;
        },
        methods: {
        },
        setup() {
        },
        watch: {},
    };
</script>

