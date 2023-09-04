import http from "./http.js";

async function createNewRequest(type, url, params) {
    params = params ? params : {};
    return await http[type](url, {
        params: {
            ...params
        }
    });
}

export default createNewRequest;



