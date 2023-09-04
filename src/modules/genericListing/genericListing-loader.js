import { createApp } from "vue";
import genericListing from "./genericListing.vue";

export function load() {
    const selector = "genericListing";

    // Is the custom Vue root element in the DOM?

    if (!document.querySelector(selector)) {
        return;
    }

    // Create a new Vue app with the imported component

    const app = createApp(genericListing, {
        //props: [...window[selector]]
        props: { ...window[selector] }
    });
    app.mount(selector);
}
