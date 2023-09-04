// Import component to be loaded like the following
// import { load as loadExample } from './modules/<ModuleName>/<ComponentName>Loader.js';
import { load as sampleModule } from "./modules/sampleModule/sampleModule-loader";
import { load as genericListing } from "./modules/genericListing/genericListing-loader.js";
import { load as newGenericGallery } from "./modules/NewgenericGallery/genericGalleryLoader";
import { load as MultipleGalleryLoader } from "./modules/NewgenericGallery/MultipleGalleyLoader";
import { load as searchPage } from "./modules/searchPage/SearchPageLoader";
import { load as GenericForm } from "./modules/GenericForm/GenericFormLoader";
import { load as SocialMediaSharing } from "./modules/SocialMediaSharing/SocialMediaSharing-Loader";


export function loadComponents() {
    // call the loading function here
    // loadExample()
    sampleModule();
    genericListing();
    newGenericGallery();
    MultipleGalleryLoader();
    searchPage();
    GenericForm();
    SocialMediaSharing();
}
