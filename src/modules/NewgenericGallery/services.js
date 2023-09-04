import axios from "axios";
import createNewRequest from "../../services/createNewRequest";
import httpConstants from "../../services/http-constants";

export function callGenericGallery(url, onSuccess, onError) {
    createNewRequest(httpConstants.HttpTypes.GET, axios.defaults.baseURL + url)
        .then((response) => {
            onSuccess(response);
        })
        .catch((error) => {
            onError(error);
        });
}
