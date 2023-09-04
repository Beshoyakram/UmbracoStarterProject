import { createApp } from "vue";
import sampleModule from "./sampleModule.vue";

export function load() {
    const selector = "sampleModule";

    // Is the custom Vue root element in the DOM?

    if (!document.querySelector(selector)) {
        return;
    }

    // Create a new Vue app with the imported component

    const app = createApp(sampleModule, {
        //props: [...window[selector]]
        props: { ...window[selector] }
    });
    app.mount(selector);
}
