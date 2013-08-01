# Long term pending tasks

* [Integrate AmplifyJS and Knockout](http://craigcav.wordpress.com/2012/05/16/simple-client-storage-for-view-models-with-amplifyjs-and-knockout/) to save draft data
* Add Knockout validation.  Groundwork is there, but view models need appropriate validation groups _and_ markup needs error message containers.
* Migrate vm.sethaul.js and vm.tripinfo.js from knockout.viewmodel plugin back to knockout mapping plugin.
* As an alternative to the above, create a bug against knockout.viewmodel for performance issue in that plugin.
* Some users have reported performance issues with the GEN-1 sighting model.  Not only are there client issues, but too many records is also a server-side problem
  (Microsoft limits JSON deserialization to prevent DOS attacks).
* Edit forms have used copy/paste inheritance for the warning on navigating away from a dirty page.  This should be extracted into a common script.
* Add DCT rating/notes functionality
* It would be great to have a better solution for the application root directory in App/datacontext.js.  The current solution is to create an attribute
  on each page (id='applicationHome') that is set in the shared Razor template.  Unfortunately, this makes the datacontext URL's dynamic
  (at least as far as Amplify is concerned).  See the following resources for a potential fix involving generating JS via Razor
  [First link](http://blog.pmunin.com/2013/04/dynamic-javascript-css-in-aspnet-mvc.html)
  [Second link](http://stackoverflow.com/questions/16092473/dynamically-generated-javascript-css-in-asp-net-mvc)
  [Third link (CSS, but the concept is similar)](http://www.codeproject.com/Articles/171695/Dynamic-CSS-using-Razor-Engine)
* The WebApi area controllers don't have any CORS support.  Allegedly, the upcoming version of WebApi will 
  include two distinct CORS implementations, one that is configured via attributes and another that can read configuration from a database.
