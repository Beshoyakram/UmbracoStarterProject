import { createApp } from 'vue';
import SocialMediaSharing from "./SocialMediaSharing.vue";
import VueSocialSharing from 'vue-social-sharing';

export function load() {
    const selector = "SocialMediaSharing";
  // Is the custom Vue root element in the DOM?

  if (!document.querySelector(selector)) {
    return;
  }

    // Create a new Vue app with the imported component
    // eslint-disable-next-line no-debugger
    const app = createApp(SocialMediaSharing, {
        props: { ...window[selector] }
    })
    app.use(VueSocialSharing);
    app.mount(selector)
}
