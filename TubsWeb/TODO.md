# Long term pending tasks

* [Integrate AmplifyJS and Knockout](http://craigcav.wordpress.com/2012/05/16/simple-client-storage-for-view-models-with-amplifyjs-and-knockout/) to save draft data
* Add Knockout validation.  Groundwork is there, but view models need appropriate validation groups _and_ markup needs error message containers.
* Migrate vm.sethaul.js and vm.tripinfo.js from knockout.viewmodel plugin back to knockout mapping plugin.
* As an alternative to the above, create a bug against knockout.viewmodel for performance issue in that plugin.
* Some users have reported performance issues with the GEN-1 sighting model.  Not only are there client issues, but too many records is also a server-side problem
  (Microsoft limits JSON deserialization to prevent DOS attacks).
* Edit forms have used copy/paste inheritance for the warning on navigating away from a dirty page.  This should be extracted into a common script.
