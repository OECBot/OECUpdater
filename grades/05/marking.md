CSCC01 Deliverable 5 Marking Guide
-----------------------------------

Total: 80/110

Project Management:  __________ (17/20%)

  Release plan:
    - each release corresponds to end of Sprint
    - project fully laid out:
      for each release date, list of user stories to implement
    - project velocity specified and used
    - highest priority user stories first modulo dependencies
    - reasonable goals for each release

  Product Backlog:
    - user stories follow the correct format.
    - user stories, together, reflect all the user requirements (from
      all personas).
    - user stories correspond to actual requirements (no "invented"
      features).
    - user stories contain enough information for devs to estimate how
      long it would take to implement it.
    - each user story addresses one specific requirement (no "world
      peace" stories).


  Sprint Backlog(s):
     - user stories taken from the Product Backlog
     - estimated costs
     - estimated values


  Sprint Plan(s):
    - user stories divided into tasks when appropriate.
    - each Sprint Plan contains all the necessary information:
        who is working on which task on which day
    - sum of costs of user stories in the sprint = project velocity
    - good planning decisions


  Use of Task Board:
    - correct format of task boards
    - snapshots in the begginning and end of each Sprint are provided


  Use of Burndown Charts:
    - snapshots in the begginning and end of each Sprint are provided
    - correctness of burndown charts


  Use of Repo:
      (including good use of branching from now on!)

  Use of communication tools:
     - evidence of using communication tools of their choice effectively

[comment]
- No dates on the release plan (-1)
- binaries and libraries have pushed to Github (-2)

Design and Implementation:  __________ (40/50%)

   System Design:
      - good modular design
      - general and easy to extend
      - it is clear how the current implementation is going to be
        extended should the requirements evolve

   Implementation:
     - 0 marks if it doesn't run
     - all of the features are fully implemented
     - quality and maturity of code

[comment]
- design documents does not match the code (-5)

[project]
- GUI-based. Good interface
- app is extensible (plugin-based)
- capable of fetching both database
- good matching algorithm
- app is customizable (workers, scheduler)
- bugs

Verification and Validation:  __________ (10/20%)

     - quality of unit tests:
         - good coverage
	 - no redundancy
     - code review activities and outcomes:
         - each review follows a checklist (not necessarily the example one, but the same checklist for all)
	 - each review is thorough
	 - evidence of addressing the outcomes of the review
	 - the video demonstrates an effective process used for the review/collection meeting
     - validation activities and outcomes:
          - evidence of feedback from the user
	  - evidence of addressing feedback from the user

[comments]
- Good code review
- Overall, bad testing strategy. Each feature branch has tests on a separate branch. These tests are never pushed to the master branch. Therefore, there is no way to re-run all tests at once. This is very hard to manage as the application grows and needs to be maintained.

Report: __________ (10/10%)

  - well-presented, is easy to read and to navigate
  - spelling and grammar
  - looks professional
  - quality of the README file

[commment]
- good

Interview: ________ (3/10%)

1) Checkout, build, run [1/2]:

=> good however the student did not tell to create a fork of Hanno repository initially

2) Demo [0/2]:

=> demo makes no sense

- login with github login/pasword and enter the name of the repo with hanno's database
- increase the number of worker to 1 when doing the sync because 0 (default) won't produce anything
- pull requests are automatic (1 pull-request per system), branch named with system name + random string to avoid conflicts
- pull requests are automatic and must be viewed from the app
- from the interface you can accept/reject pull requests that will close them on the forked repo
- the diff does not show the changes (bulk)
- pushing to pull request to Hanno's database is done manually through Github

3) Project Management [0/2]:

=> makes no sense

- Sprint 5, 6 and (small) 7
- middle sprint 5 did a change in product backlog, did a re-plan but did not affect the sprint backlog (that makes no sense)
- Sprint 5 (replan): detecting conflicts from fetched data, pull requests
- Sprint 6 (new product backlog*): UI (not completed)
- Sprint 7: UI

* new backlog:
- deleted features: history
- split an epic (UI) into: pull request UI, plugin UI, bot management

4) Code reviews [2/2]:

=> good

- 1 code review: at the end sprint 6
- only the new feature: manual code inspection
- findings: duplicate code, dead code, documentation, unclosed branch
- In sprint 7: documentation, closed few branches

5) Testing and Validation [0/2]:

=> The test strategy makes no sense

- unit tests are on a separate branch (each feature has its own test branch)
- integration tests and units tests are mixed
- functional tests are main files on the test branch
- overall, no way to re-run all tests


