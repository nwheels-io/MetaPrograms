# Intentions-as-code

We develop applications by translating our intentions (or requirements) into code. Often, a signle intention is projected into many pieces of code across multiple layers, tiers, and services. For example, take a single requirement like "user's email must be unique". Its implementation can be scattered across:

- database scripts
- domain logic
- RESTful API
- UI cues and validations
  - multiplied by the number of UI platforms involved (web/mobile/desktop...)
- IVR (interactive voice response) flows
- tests that cover all of the above

Traditionally, we write and maintain these by ourselves. However, they are just mechanical derivatives of our original intentions, and can be produced by some kind of automated mechanism. For that, the mechanism must be able to:

- understand the intentions, including variations in every occurrence
- provide templated implementations of the intentions, in every layer involved

It's actually simpler than it sounds, at least when enterprise applications are considered. From my experience, commonality in both functional and non-functional requirements here is dramatically higher than variability. Which means, we can compile a catalog of common intentions (or requirements), together with their templated implementations. 

Thus, we only need to code and maintain the intentions, not their implementations. Our code is now easily abstracted from any concrete frameworks and products. It expresses the distilled concepts of the application, clear of any technical details, which don't clutter our codebase any more. It   

Another important consideration. We'd like to abstract 


