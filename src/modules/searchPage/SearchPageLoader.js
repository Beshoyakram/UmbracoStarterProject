
import { createApp } from 'vue';
import searchPage from './SearchPage.vue';

export function load() {
    const selector = 'searchPage';

    // Is the custom Vue root element in the DOM?

    if (!document.querySelector(selector)) {
        return;
    }

    // Create a new Vue app with the imported component
    const app = createApp(searchPage, {
        //props: [...window[selector]]
        props: { ...window[selector] },
    })
    app.mount(selector)
}
