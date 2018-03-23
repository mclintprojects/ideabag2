<template>
    <div class="appContainer">
        
            <div class="form-group ">
                <label for="emailTb ">Email address</label>
                <input v-model="formData.email " id="emailTb " type="email " class="form-control " placeholder="Enter email ">
            </div>
            <div class="form-group ">
                <label for="passwordTb ">Password</label>
                <input v-model="formData.password " type="password " class="form-control " id="passwordTb " placeholder="Password ">
            </div>
            <button @click="loginUser" class="appBtn">Submit</button>
        
    </div>
</template>

<script>
export default {
    data() {
        return {
            formData: {
                email: '',
                password: ''
            }
        };
    },
    methods: {
        loginUser() {
            this.$store.dispatch('loginUser', this.formData);
        }
    }
}
</script>

<style scoped>
.container-fluid {
    margin-top: 60px;
}

label {
    color: var(--primaryText);
    font-family: 'Roboto', sans-serif;
}

.appContainer {
    padding: 16px;
    padding-top: 50px;
}

.form-control {
    border: solid 0px transparent;
    border-radius: 2px;
    height: 40px;
    font-weight: normal;
}

.form-control:focus {
    box-shadow: 0px 0px 0px transparent;
}
</style>

