<template>
    <div style="border:1px solid;padding:10px;margin:10px;">
        <a style=""
           :href="url"
           v-html="title ? highlightText(limitedTitle, searchKeyword) : title"
           :title="title">
        </a>

        <p class="search-result__description"
           v-html="
      description ? highlightText(limitedDisc, searchKeyword) : description
    "
           :title="description"></p>
    </div>

</template>
  
  <script>
export default {
  props: ["title", "description", "url"],
  data() {
    return {
      highlightedText: "",
      searchKeyword: "",
      limitedDisc: "",
      limitedTitle: "",
    };
  },
  methods: {
    highlightText(word, query) {
      if (query.length > 0) {
        let myReg = new RegExp("(" + query.replaceAll(" ", ")|(") + ")", "gim");
        word = word.replaceAll(myReg, (highlightWord) => {
          return '<span class="highlighted">' + highlightWord + "</span>";
        });
        return word;
      } else {
        return word;
      }
    },
    // function used to get the position of search keyword in whole text and return trimmed text including search keyword
    getTextWithSearchWord(text) {
      if (!text.includes(this.searchKeyword)) {
        return text;
      }
      let limitedText = text.substring(0, 155);
      const isSearchWordExist = limitedText.includes(this.searchKeyword);
      if (isSearchWordExist) {
        // in case search keyword exist in trimmed text
        return text;
      } else {
        const searchWordPosition = this.description.indexOf(this.searchKeyword);
        let textWithSearchWord = "";
        let addedTextToEnd = "";
        limitedText = text.substring(0, 120);
        addedTextToEnd = this.description.substring(searchWordPosition);
        textWithSearchWord = `${limitedText} ...${addedTextToEnd}`;
        return textWithSearchWord;
      }
    },
    getTitleWithSearchWord(title) {
      if (!title.includes(this.searchKeyword)) {
        return title;
      }
      let limitedTitle = title.substring(0, 50);
      const isSearchWordExist = limitedTitle.includes(this.searchKeyword);
      if (isSearchWordExist) {
        return title;
      } else {
        const searchWordPosition = this.title.indexOf(this.searchKeyword);
        let titleWithSearchWord = "";
        let addedTextToEnd = "";
        limitedTitle = title.substring(0, 30);
        addedTextToEnd = this.title.substring(searchWordPosition);
        titleWithSearchWord = `${limitedTitle} ...${addedTextToEnd}`;
        return titleWithSearchWord;
      }
    },
  },
  mounted() {
    const urlParams = new URLSearchParams(window.location.search);
    this.searchKeyword = urlParams.get("searchkeyword");
    if (this.description) {
      this.limitedDisc = this.getTextWithSearchWord(this.description);
    }
    this.limitedTitle = this.getTitleWithSearchWord(this.title);
  },
};
</script>
  