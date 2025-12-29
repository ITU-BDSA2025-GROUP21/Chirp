---
title: 'Chirp!_Project_Report'
subtitle: 'ITU BDSA 2025 Group 21'
author:
- "Alfred Damgaard <alfd@itu.dk>"
- "Alfred Schrøder Oldin <aold@itu.dk>"
- "Andreas John-Holaus <andjo@itu.dk>"
- "Anton Thejsen <antt@itu.dk>"
- "Noah Leerbeck Van Wagenen <noav@itu.dk>"
- "Philip Bay Quorning <phqu@itu.dk>"
numbersections: true
---

# Chirp Project Report
## ITU BDSA 2025 Group 21

Table of contents


[TOC]

# Design and architecture of Chirp!

## Domain model

## Architecture -- In the small

Write about onion structure and insert picture of it...

## Architecture of deployed application

Write about how we deply, also insert a small model that depicts the git, azure and client fucntions... 

## User activities
When a user enters the website for the first time they will start off on the public timeline. Here they will have access to the entire public timeline as well as each seperate users timeline.
While not logged in a user cannot post cheeps, follow authors or like/dislike cheeps, to solve this a user must access either the login page or the register page. From here the user can either create a new profile or login respectively.
The login and registration can also be done using GitHub.

When a user has sucssesfully logged in they will be redirected to the public timeline once again, this time logged in of course.
While here a user can read just the same as if they were not logged in, however this time the user is also able to post cheeps, follow other authors and like/dislike their cheeps.

From the public timeline a few new pages have opened up, the first one is the users own timeline, here a collection of the users previous posts can be read.
Second is the account page, here all the information about ones own profile can be read, this includes an email, password etc.
Using this page it is also possible to change ones own profile picture, as well as deleting or downloading all your personal data. Once this is done you will be logged out and sent back to the public timeline, this time without being logged in.

The following UML chart represents the available "options" a user has depending on where on the website they are located, and whether they're logged in or not.

![Untitled Diagram.drawio](https://hackmd.io/_uploads/SkUhOOTQbe.png)


## Sequence of functionality/calls through chirp


# Process

## Build, test, release and deployment
**Nok for meget tekst.
Lav uml diagram over workflow, og bind teksten op i mod den.**

The development process followed a trunk-based development approach, combined with CI (continoues integration) practices. The developmen tprocess ensured that changes were kept small, with frequent integrations into the main branch. The workflow consited of a 

#### Build Process and issues
The trunk-based git branching strategy entails having a main integration branch and having short lived feature branches. Development starts with the creation of a GitHub issue, the structure of which is based on a user story containing the following:
 - A description of the problem or feature
 - Acceptance criteria defining when the task is considered complete.

The purpose of a GitHub issue is to both grant the developers a shared overview of what needs doing, and define the actual problem, while keeping the overall definiton non-technical and purely user-experience or stakeholder oriented. Each issue should aim to be consice enough to be implemented, reviewed and merged through a single short-lived feature branch. This is not a requirment, but compliments the idea of trunk-based Git branching strategy. For each issue, a feature branch is created with a name that reflects the issue or the feature being worked on. 

#### Test Process
Testing is a central part of the workflow, providing a human error filter that ensures new features don't accidentally break existing functionality. In the workflow, it is used as a gatekeeper before merging code into the main branch. For the testing process we relied on a combination of:
 - Unit tests for isolated logic 
 - Integration tests to verify interactions between components
 - End-to-end tests for verifying complete user flows throught the application.

When developing new features, tests would be added, covering the introduced functionality. Adding tests is a requirment in the workflow, as new features being continuously introduced could lead to overlapping code that unintentionally breaks existing functionality. By ensuring feature-specific tests, the code is future proofed for incomming additions. 

End-to-end tests were implemented using Playwright, a framework for automating browser input. Playwright allows the application to be tested in a browser enviorment. These test simulated user interactions with the UI, and then validate the behavior or output. This tests the entire application stack, all the way from frontend UI to backend services. 

These tests combined helped ensure that corner cases, regressions and unintended side effects were identified early on in the process. They reduce the likelihood of errors reaching the main branch. To enforce this constistently, the testing process was integrated into automated GitHub workflows, which are described in the following section.


#### Github Workflows
The project used multiple GitHub Actions workflows that automated building, testing, relasing and deploying the application. The workflows acted as gates by verifying that changes followed procedure. One workflow was responsible for automaticly building and testing the application. This was triggered on pushes and pull requests that targeted the main branch. Another workflow handled deployment to Azure, also triggered by pushes and pull requests. In addition, there were also a dedicated publish workflow used for versioning releases. Together these workflows automated alot of processes that would otherwise cost manual overhead.



#### Release Process
Releases where handled through merges into main, using pull requests. Once a feature branch passed the automated tests, had been reviewed by peers and checked for code quality, the branch is considered stable enough for merge. The pull request acts as a pre-release checkpoint ensuring only verified and reviewed changes were integrated

This process ensured that the main branch always was in a deployable state, which is one of the core principle of trunk-based developmnent.

Versioning was handled through pull requests. Each merged pull request represented a new version of the application. This meant that changes were released incrementally and remained traceable.

#### Deployment
The application is deployed using Azure Web App. As stated in the previous section, deployment is handled through GitHub Actions workflow, which triggers on pushes to the main branch.

## Team work
"make an illustration of trunk-based develpoment"

During our development of Chirp we have followed a trunk-based development workflow. Short-lived branches were created for individual tasks and features, each branching from the main trunk. Development was carried out on these branches with frequent, small commits documenting progress. Once a task was completed, a pull request was opened targeting the main branch. Team members reviewed the changes to ensure correctness and adherence to project standards. After approval, branches were merged into main, and the corresponding issues were closed. This workflow allowed us to maintain a stable main branch while supporting rapid development and collaboration.

## How to make Chirp! work locally

# Ethics


## License
MIT License

Copyright (c) 2025 ChirpGroup21

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## LLMs, ChatGPT, CoPilot, and others
During the development of Chirp, we made limited and deliberate use of Large Language Models (LLMs) as a supportive learning and reference tool. LLMs were primarily used to help us understand various .NET libraries, third-party frameworks, and other unfamiliar technical aspects encountered throughout the project. This usage was restricted to conceptual explanations, clarification of documentation, and general guidance, and did not involve the direct generation of application logic or implementation-specific code. LLMs were not used to produce raw application code, nor was any LLM-generated code directly incorporated into the final solution. 
Additionally, Copilot was used to assist in generating initial drafts of code documentation. These drafts were then reviewed, refined, and adjusted by the project group to ensure correctness, clarity, and alignment with the actual implementation. Overall, LLMs functioned as a supplementary aid rather than a development tool, and all design decisions, implementation work, and final outputs remain the result of our own work.ase, and deployment

## Team work

## How to make _Chirp!_ work locally

## How to run test suite locally

# Ethics

## License

## LLMs, ChatGPT, CoPilot, and others