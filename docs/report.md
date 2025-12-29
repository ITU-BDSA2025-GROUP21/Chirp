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
Model 1:
![Illustration of the _Chirp!_ data model as UML class diagram.](/docs/images/DomainModel.png)
As illustrated, the Domain Model consists of four domain entities: **Author**, **Cheep**, **Like**, and **UserFollow**.
The model integrates **ASP.NET Identity**, with **Author** inheriting from `IdentityUser` to provide built-in authentication and authorization, including email and password management.

### Author

The central entity of the domain, representing a user of the system.

**Responsibilities and properties:**
- User name
- Account creation date
- Path to a custom profile picture
- Collection of authored cheeps
- Collection of followers
- **Karma**: a likeness score representing user reputation

In addition, `Author` inherits all properties from `IdentityUser`, such as email and password handling.

### Cheep

Represents a post created by an author.

**Properties:**
- **CheepId**: Unique identifier
- **Author**: Reference to the author who created the cheep
- **Text**: The posted message
- **TimeStamp**: Time of creation
- **Likes**: Collection of `Like` entities associated with the cheep

### Like

Represents a like or dislike issued by an author on a cheep.

**Properties:**
- **AuthorId**: Unique identifier of the author issuing the like
- **CheepId**: Unique identifier of the liked cheep
- **LikeStatus**: Integer indicating like (`1`) or dislike (`-1`)
- **Author**: Reference to the liking author
- **Cheep**: Reference to the liked cheep

### UserFollow

Represents a follow relationship between two authors.

**Properties:**
- **FollowerId**: Unique identifier of the following author
- **FolloweeId**: Unique identifier of the followed author
- **Follower**: Reference to the author who follows
- **Followee**: Reference to the author being followed
- **TimeStamp**: Time when the follow occurred


## Architecture -- In the small
Model 2:
![Architecture](/docs/images/Architecture.png)

As illustrated above, this solution follows the Onion Architecture principle, where all dependencies point inward toward the domain. The goal is to keep business logic isolated from technical and presentation concerns.

### Domain Layer

The core of the application.

- Domain entities
- Repository interfaces
- Business rules

**Characteristics:**
- Dependency-free
- Independent of frameworks and infrastructure
- Represents pure business logic

### Application Layer

Coordinates use cases and application workflows.

- Application services and use cases
- Service interfaces
- Data Transfer Objects (DTOs)
- Mapping between domain entities and DTOs

**Characteristics:**
- Depends only on the Domain layer
- Contains no infrastructure or UI code

### Infrastructure Layer

Handles technical and external concerns.

- EF Core DbContext and migrations
- Repository implementations
- Framework- and third-party-dependent service implementations (e.g. ASP.NET Identity)

**Characteristics:**
- Implements interfaces from inner layers
- Depends on Domain and Application layers

### User Interface Layer

Handles presentation and user interaction.

- Razor Pages and Page Models
- Dependency Injection setup
- Application startup and configuration

**Characteristics:**
- Depends on the Application and Infrastructure layer
- Contains no business logic

### Test Layer

Validates the system across all layers.

- Unit, Integration, End-to-End, and UI tests
- Not part of the runtime architecture

## Architecture of deployed application

Write about how we deply, also insert a small model that depicts the git, azure and client fucntions... 

# User activities
When a user enters the website for the first time they will start off on the public timeline. Here they will have access to the entire public timeline as well as each seperate users timeline.
While not logged in a user cannot post cheeps, follow authors or like/dislike cheeps, to solve this a user must access either the login page or the register page. From here the user can either create a new profile or login respectively.
The login and registration can also be done using GitHub.

When a user has sucssesfully logged in they will be redirected to the public timeline once again, this time logged in of course.
While here a user can read just the same as if they were not logged in, however this time the user is also able to post cheeps, follow other authors and like/dislike their cheeps.

From the public timeline a few new pages have opened up, the first one is the users own timeline, here a collection of the users previous posts can be read.
Second is the account page, here all the information about ones own profile can be read, this includes an email, password etc.
Using this page it is also possible to change ones own profile picture, as well as deleting or downloading all your personal data. Once this is done you will be logged out and sent back to the public timeline, this time without being logged in.

The following UML chart represents the available "options" a user has depending on where on the website they are located, and whether they're logged in or not.

Model 3:
![Untitled Diagram.drawio](https://hackmd.io/_uploads/SkUhOOTQbe.png)


## Sequence of functionality/calls through chirp


# Process

## Build, test, release and deployment
Figure X, illustrates the flow of activities in the GitHub Actions workflows used to build, test, release, and deploy the application.
When a pull request is opened an automated CI (continuous integration) workflow is triggered. This workflow restores dependencies, builds the application and executes the automated tests. The purpose of this step is to ensure that changes are valid and does not introduce compile errors or any other errors. If the build or tests fail, the workflow stops and the pull request cannot be merged.
In addition to automated testing and compiling, CodeFactor is used as a code analysis tool. Quality issues are identified and has to be resolved before a pull request could be merged. If the workflow completes successfully, the pull request can be merged into the main branch. 
Once changes are merged into the main branch a deployment workflow is triggered. This workflow deploys the application to an Azure Web App. As a result, the production environment is continuously updated with tested and reviewed changes.
In cases where the merge is tagged with a version, an additional publish workflow is executed. This workflow builds and tests the application, generates versioned release artifacts and creates a corresponding GitHub release. If no version tag is present, the process ends after deployment.


## Team work
Model 4:
![trunkbased stuff](https://hackmd.io/_uploads/HkD-Kbl4Zl.png)

During our development of Chirp we have followed a trunk-based development workflow. Short-lived branches were created for individual tasks and features, each branching from the main trunk. Development was carried out on these branches with frequent, small commits documenting progress. Once a task was completed, a pull request was opened targeting the main branch. Team members reviewed the changes to ensure correctness and adherence to project standards. After approval, branches were merged into main, and the corresponding issues were closed. This workflow allowed us to maintain a stable main branch while supporting rapid development and collaboration.

Model 5:
![CI_CD_flow](https://hackmd.io/_uploads/Hk9vwgeVZl.png)


Model 5 is a workflow UML that shows the workflow process, used throughout the development. This model was created early on in the project, to create a uniform and structured way of developing features. 
Development begins with Github issue with a structure based on a user story, containing a description and acceptance criteria. This is kept non-technical and purely user-experience or stakeholder oriented. Each issue should aim to be consice enough to be implemented, reviewed and merged through a single short-lived feature branch. This is not a requirment, but compliments the idea of trunk-based Git branching strategy. For each issue, a feature branch is created with a name that reflects the issue or the feature being worked on. When the feature has been developed and tests are added and passed, a pull request is created for peers to review. The reviewer should ensure that the acceptance criteria and SOLID principles are covered. Once the Github workflow finishes, the related issue is closed.

### Pair Programming and Commit Attribution
During the project, a significant portion of the development was carried out through pair programming sessions using Visual Studio Live Share. In these sessions, two or more team members worked together simultaneously on the same code, discussing design decisions and implementing solutions collaboratively.

Although Git supports the use of Co-Authored-By tags, these were not applied consistently or always correctly during development. As a result, the commit history does not fully reflect the actual distribution of contributions, as many commits represent collaborative work rather than individual effort.
The commit log should therefore be understood as a technical record of changes, not as an exact measure of individual contribution. Knowledge sharing and joint problem solving were intentional parts of the development process and played a significant role in the final solution.
This is also reflected in a large amount of commits, where only 1 is credited for the commit. Examples include: [133f7ee](https://github.com/ITU-BDSA2025-GROUP21/Chirp/commit/133f7ee19a82129298acbcb4b251df8f84f7f7a0), [3cba4c0](https://github.com/ITU-BDSA2025-GROUP21/Chirp/commit/3cba4c095730e68b9b16332dcb9defd25c327c8e), [386f3b1](https://github.com/ITU-BDSA2025-GROUP21/Chirp/commit/386f3b1ef69797b272527cc5400a18c103f4abd1), [d6e8216](https://github.com/ITU-BDSA2025-GROUP21/Chirp/commit/d6e8216ef668307627530742d961dc456e91d92d)...

## How to make Chirp! work locally



## Running Test Suite locally




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
During the development of Chirp, we made limited and deliberate use of Large Language Models (LLMs) as a supportive learning and reference tool. LLMs were primarily used to help us understand various .NET libraries, third-party frameworks, and other unfamiliar technical aspects, providing conceptual explanations, clarification of documentation, and general guidance. They were not used to generate raw application code, and no LLM-generated code was incorporated into the final solution.

Copilot was also used as an initial aid for the documentation process, offering preliminary suggestions and structure. These suggestions were carefully reviewed and fully rewritten or adapted by the project group to ensure accuracy, clarity, and alignment with the implementation. Overall, LLMs served as a supplementary resource rather than a development tool, with all design decisions, implementation work, and final outputs produced entirely by the project team.