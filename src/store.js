import { createStore } from 'vuex'
import authStore from './modules/auth/store'

const store = createStore({
    // combine multiple stores
    modules: {
      auth: authStore,
    }
})

export default store