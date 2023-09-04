<template>
    <div class="search-page">
        <div class="main-container">
            <div class="search-container">
                <form @submit.prevent="searchinputValueChanged">
                    <p>{{ props.SearchFor }}</p>
                    <div class="search-bar">
                        <input type="text"
                               v-model="mainSearchKeyword"
                               :placeholder="props.SearchForLabelPlaceholder" />
                        <i class="icon-search-icon"
                           @keyup.enter.prevent="searchinputValueChanged"
                           @click.prevent="searchinputValueChanged"></i>
                    </div>
                </form>
            </div>

            <div class="search-results-container"
                 v-if="detailsData.searchResults != null && isApiCalled">
                <div class="search-results">
                    <div class="search-result"
                         v-if="detailsData.searchResults?.length > 0"
                         :key="attmp">
                        <section class="search-result"
                                 v-for="(searchResult, index) in detailsData.searchResults"
                                 :key="index"
                                 :class="{
                'search-result--category-selected':
                  generatedQuery.categoryId != '',
              }">
                            <SearchCard v-if="detailsData.searchResults?.length > 0"
                            :title="searchResult.title"
                            :description="searchResult.brief"
                            :url="searchResult.url"
                            :key="attmp"></SearchCard>
                        </section>
                    </div>
                </div>
            </div>
            <div class="row"
                 v-if="!isLoading && detailsData.searchResults?.length == 0">
                <div class="col-md-12 genericListing--min-height-section">
                    <div class="genericListing__empty-message generic-empty-message">
                        <img src="../../assets/gold-no-message.svg"
                             :alt="props.noResultFound" />
                        <h3>{{ props.noResultFound }}</h3>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12" v-if="isLoading">
                    <div class="genericListing__generic-card-contents-loader">
                        <Loader :show="isLoading" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <div id="generic-observer"></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import axios from "axios";
    import Loader from "../global/components/loader/Loader.vue";
    import SearchCard from "./SearchCard.vue";
    let getSearchParams;
    export default {
        components: {
            Loader,
            SearchCard,
        },
        data() {
            return {
                props: {},
                isInitedBefore: false,
                isLoading: false,
                dataFinished: false,
                fullySearchWithLoadOnScroll: true,
                detailsData: {},
                mainSearchKeyword: "",
                generatedQuery: {
                    pageNumber: 1,  
                    Culture: "",
                    searchKeyWord: "",
                    categoryId: "",
                },
                nmberOfPages: 0,
                isApiCalled: false,
                attmp: 0,
                filteredList: [],
            };
        },
        methods: {
            callApi() {
                this.isLoading = true;
                this.isInitedBefore = false;
                axios
                    .get(
                        `${window.location.origin}/umbraco/api/Search/index?pageNumber=${this.generatedQuery.pageNumber}&culture=${this.generatedQuery.Culture}&text=${this.generatedQuery.searchKeyWord}`,
                        this.generatedQuery
                    )
                    .then((res) => {
                        if (res.data.data?.length == 0 && this.generatedQuery.pageNumber == 1) {
                            this.detailsData.searchResults = [];
                        } else if (res.data.data?.length != 0) {
                            this.isApiCalled = true;
                            this.isLoading = false;
                            if (this.generatedQuery.pageNumber == 1) {
                                this.detailsData.searchResults = res.data.data;
                            } else {
                                this.detailsData.searchResults = this.detailsData.searchResults.concat(res.data.data);
                            }
                        }

                        this.isLoading = false;
                        this.isInitedBefore = true;
                        this.dataFinished = true;
                    })
                    .catch(() => {
                        document.querySelector("#snackbar").innerText =
                            this.props.somethingWentWrongLabel;
                        document.querySelector("#snackbar").classList.add("show");
                        setTimeout(function () {
                            document.querySelector("#snackbar").classList.remove("show");
                        }, 3000);
                        this.isLoading = false;
                    });
            },
            checkObserverTrigger() {
                const options = {
                    threshold: 1.0,
                };
                const checker = document.querySelector("#generic-observer");
                const infiniteScroll = (entries) => {
                    entries.forEach((entry) => {
                        if (entry.isIntersecting && this.isInitedBefore) {
                            if (
                                this.detailsData != "undefined" &&
                                this.detailsData.searchResults?.length != 0 &&
                                this.generatedQuery.pageNumber >= 1 &&
                                this.dataFinished &&
                                this.fullySearchWithLoadOnScroll
                            ) {
                                setTimeout(() => {
                                    this.generatedQuery.pageNumber++;
                                    this.callApi();
                                }, 500);
                            }
                        }
                    });
                };

                const observer = new IntersectionObserver(infiniteScroll, options);
                observer.observe(checker);
            },
            searchinputValueChanged() {
                if (this.mainSearchKeyword.trim().length >= 3) {
                    //Set search Params here
                    getSearchParams = new URLSearchParams(window.location.search);
                    getSearchParams.set("searchkeyword", this.mainSearchKeyword);
                    let newurl =
                        window.location.protocol +
                        "//" +
                        window.location.host +
                        window.location.pathname +
                        "?" +
                        getSearchParams.toString();
                    window.history.replaceState({ path: newurl }, "", newurl);
                    this.generatedQuery.searchKeyWord = this.mainSearchKeyword;
                    this.setHeaderSearchInputText();
                    this.dataFinished = false;
                    this.generatedQuery.pageNumber = 1;
                    this.detailsData.searchResults = [];
                    this.generatedQuery.categoryId = "";

                    this.callApi();
                }
            },
            getSearchTextQueryString() {
                getSearchParams = new URLSearchParams(window.location.search);
                let searchText = getSearchParams.get("searchkeyword");
                if (
                    searchText != null &&
                    searchText != "" &&
                    searchText.trim().length >= 3
                ) {
                    this.generatedQuery.searchKeyWord = searchText;
                    this.mainSearchKeyword = searchText;
                    //Set Header search input text
                    this.setHeaderSearchInputText();
                    this.callApi();
                } else {
                    this.mainSearchKeyword = searchText;
                    if (!this.isInitedBefore) {
                        //TODO update Header once we have it's value
                        //document.querySelector(".navbar-form__group--desktop input").value =
                        //    this.mainSearchKeyword;
                        //document.querySelector(".navbar-form--sm input").value =
                        //    this.mainSearchKeyword;
                        this.detailsData.searchResults = [];
                    }
                }
            },
            setHeaderSearchInputText() {
                //TODO , this should be filled once we have header
                //document.querySelector(".navbar-form__group--desktop input").value =
                //    this.generatedQuery.searchKeyWord;
                //document.querySelector(".navbar-form--sm input").value =
                //    this.generatedQuery.searchKeyWord;
            },
        },
        mounted() {
            this.props = this.$attrs.props;
            this.generatedQuery.Culture = this.props.lang;
            this.checkObserverTrigger();
            this.getSearchTextQueryString();
        },
        watch: {},
    };
</script>


