<template>
    <div id="toolbar" class="navbar navbar-fixed-top">
        <img v-if="!isRootComponent" @click="navigateAway" id="backBtn" src="/src/assets/ic_arrow_back_white_24px.svg" />
        <h4>IdeaBag 2 (Beta)</h4>
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
        },
        isRootRoute() {
            var route = this.$route.path;
            if (route == '/')
                return true;
            else
                return false;
        }
    },
    watch: {
        '$route': function(to, from) {
            this.isRootComponent = this.isRootRoute();
        }
    },
    created() {
        this.isRootComponent = true;
    }
}
</script>

<style>
.navbar {
    background-color: var(--primary);
    border-radius: 0px;
}

#toolbar {
    display: flex;
    align-items: center;
    box-shadow: 0px 2px 6px rgba(0, 0, 0, 0.1);
}

#toolbar h4 {
    margin-left: 16px;
    color: rgba(255, 255, 255, 0.8);
}
</style>


