// wwwroot/js/site.js
//
// NOTE: The global App and Toast helpers now live exclusively in
// core/app.js and core/toast.js (loaded earlier in the layout).
// This file previously re-declared `const App` / `const Toast`,
// which threw "Identifier has already been declared" in the browser
// and silently broke every script on the page (including toast
// notifications on Account pages). Kept as an empty extension point
// for future site-wide, non-module JS.

App.ready(function () {
    // Reserved for site-wide behavior that isn't specific to
    // a single page/module.
});