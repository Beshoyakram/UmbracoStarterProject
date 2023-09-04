import axios from "axios";

export default function setHttpDefaults() {
  let webbaseUrl = window.location.origin;
  axios.defaults.baseURL = `${webbaseUrl}/`;
}
