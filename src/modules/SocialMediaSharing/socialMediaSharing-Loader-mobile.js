import { createApp } from 'vue';
import socialMediaSharing from "./SocialMediaSharing.vue";
import VueSocialSharing from 'vue-social-sharing';

export function load() {
  const selector = "socialMediaSharingMobile";
  // Is the custom Vue root element in the DOM?

  if (!document.querySelector(selector)) {
    return;
  }

    // Create a new Vue app with the imported component
    // eslint-disable-next-line no-debugger
    const app = createApp(socialMediaSharing, {
        props: { ...window[selector] }
    })
    app.use(VueSocialSharing);
    app.mount(selector)
}
