
import { createApp } from 'vue';
import genericGallery from './genericGallery.vue';

export function load() {
    const selector = 'newGenericGallery';

    // Is the custom Vue root element in the DOM?

    if (!document.querySelector(selector)) {
        return;
    }

    // Create a new Vue app with the imported component
    const app = createApp(genericGallery, {
        props: { ...window[selector] }
    })
    
    app.mount(selector)
}
