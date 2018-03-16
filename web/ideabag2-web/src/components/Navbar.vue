<template>
    <div id="toolbar" class="navbar navbar-fixed-top">
        <img v-if="(!isRootComponent)" @click="navigateAway" id="backBtn" src="../assets/ic_arrow_back_white_24px.svg" />
        <h4>IdeaBag 2</h4>
    </div>
</template>

<script>
export default {
    data() {
        return {
            isRootComponent: false
        }
    },
    methods: {
        navigateAway() {
            this.$router.go(-1);
        }
    },
    watch: {
        '$route': (to, from) => {
            this.isRootComponent = window.location.hash == '#/';
            console.log('IsRoot', this.isRootComponent);
        }
    },
    created() {
        this.isRootComponent = window.location.hash == '#/';
    }
}
</script>

<style>

</style>


