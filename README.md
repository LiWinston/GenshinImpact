[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/CibnTZFQ)

# Project 2 Report

Read the [project 2
specification](https://github.com/COMP30019/Project-2-Specification) for
details on what needs to be covered here. You may modify this template as you see fit, but please
keep the same general structure and headings.

Remember that you must also continue to maintain the Game Design Document (GDD)
in the `GDD.md` file (as discussed in the specification). We've provided a
placeholder for it [here](GDD.md).

## Table of Contents

* [Evaluation Plan](#evaluation-plan)
* [Evaluation Report](#evaluation-report)
* [Shaders and Special Effects](#shaders-and-special-effects)
* [Summary of Contributions](#summary-of-contributions)
* [References and External Resources](#references-and-external-resources)


## Evaluation Plan

    Evaluation techniques: Which evaluation techniques will you use and why? What tasks will you ask participants to perform?
    Participants: How will you recruit participants? What qualifying criteria will you use to ensure that they are representative of your target audience?
    Data collection: What sort of data is being collected? How will you collect the data? What tools will you use?
    Data analysis: How will you analyse the data? What metrics will you use to evaluate your game, and provide a basis for making changes?
    Timeline: What is your timeline for completing the evaluation? When will you make changes to the game?
    Responsibilities: Who is responsible for each task? How will you ensure that everyone contributes equally?

**1. Objectives and Scope:**
   1. The purpose of this Evaluation is to present the game's current state to beta tests, gather feedbacks and insights from testers about graphics, gameplay mechanics, audio, storyline, controls and UI. In particular, we are interested to understand how players feel about environment, monster, and other elements, whether these contribute positively to game immersion, or offer insufficient engagement for players to be motivated to continue the game.
   2. furthermore, we wish to gather feedback on QOL improvement that could be implemented to enhance game feedback, sense of control, and direction to the player. In the current design, the game is open world without a direct line of progression that forces players to undertake a certain route, concerns arises whether players could successfully complete the game with minimal level of hints currently available. Or that without clear goals or directives, players would feel like a headless chicken, not knowing what to do.

**2. Evaluation Criteria:**
    
    As mentioned above, graphics, gameplay mechanics, audio, storyline, controls and UI are the main areas of focus.
   
 Graphics must satisfy the following quality,
   1. consistency - environment design is consistent in style, lighting, and fps remains stable throughout the game. 
   2. clarity - game is communiating clearly to the player what is happening in the game, players knows where to go, where they are, who is the enemy, and how much health they have
  
  Gameplay mechanics:
  1. clear goals and objectives: main goal and objectives are intutitve to underestand, players could quickly grasp what they needed to do.
  2. Rules - the rules of game is intutitive to understand, either intutively or explicitly told by in game hints. Rules explicited stated are strictly followed by gameplay mechanics
  3. leveling-up - the game is effective in introducing to the player how to level up, what they need to do, and what they will get.
  4. progression - the game is effective in communnicating to the player how it will progress, or expect what will happen if they pick a particular path.
  5. engagement - game is sufficiently engaging the players would want to continue playing.

  Audio:
  1. clarity - audio offers clear and easy to understand feedback of what happens in the game, players could rely on audio to re-confirm changes occured.
  2. immersion - BGM is suitable to the game scene, BGM adds environment vibes, and sense of pressure and urgency.

  Control:
  1. intitutive to use - controls follow RPG conventions, characters movements and abilities are clearly communicated to the player.
  2. expression space - controls offer sufficient room for player to show mastery, and maintain a level of variety.
  3. tools - game pause, exit game options are easy to use. Players knows how to access these tools.

  UI:
  1. clarity - UI is clear to communicate game state, character state. UI is non-intrusive, does not block player view, is not a hinderance to player control.
  2. practicality - UI offers practical help, including minimap, healthbar status, mana status, ability status.

**3. Evaluation Team:**
  - The evaluation team consist of 4 team members. Each with their unique responsibility.
  - A is responsible for conducting post-playthrough interview.
  - B is responsible for taking note during structured observation where participants would be invited to play the game without any guidance. The structure of the observation and interview will be mentioned in **Testing Methodologies**.
  - C is responsible for coding the notes and interviews answers obtained, and deriving themes and condensing issues from obtained material.
  - D is respsonible for compiling obtained result into a report, highlighting common issues, and formulate possible fixes to the issues highlighted.

**4. Recuriting Participants:**
   - There would be at least 7 participants for each evaluation technique, combining for a minimum of 14 participants. 
   - Recuritment methods can be snowball or direct advertising. However, recurited participants must form a pool that have somewhat equal gender balance, have age distribution similar to the gaming community, comes from a vareity of backgrounds, including but not limited to game design and graphics design.
   - Recruited participants would be rejected if they never played any RPG games; or if they are out side the age range between 14-60.
   - Participants will be asked to consent to being observed or recorded throughout their playthrough, participants will be asked whether they consent to having their recorded material and interview answers to be included in the evaluation report, data retained without consent for distribution would not be included in the evaluation report.

**5. Testing Methodology:**

  Evaluation would be conducted through one querying and one observational methods. The team have decided to use interview and structured observation to form the interview.
  
  **Structured observation :**

  this evaluation consist of instructing the participants to play through the game, the goal and the task will be explained to the player prior to starting the obseration. During the playthrough the participants must persist without any external hints or guidances from the observer.

  The observer will take notes on participants' experience with the game, as per evaluation criteria listed above. Areas where participants expressed particular dissatisfaction, or confusion are to be documented, reasons of which caused confusion can either be inferred from player reaction or complaints. Participants would be encouraged to voice their dissatisfaction outloud while they are playing the game. 

  **Semi-Structured interview :**
  
  Semi-structured interview consist of two part, the first is a set of predetermined set of questions to all participants in the same order on their experience with the initial playthrough. 
  
  The second part is a set of open questions on all aspects of the game, including but not limited to graphics, controls, environment setting, mechanics, storyling flow. The aim of these questions is to understand participant's insights on what they would have added or deleted from the game.

**6. Data Collection:**
   
   1. Video recording - we expect to obtain video recording of both the playthrough and the interviews, subject to the participant's consent. Recording may be included as part of material to the evaluation report should the participant consent for it to be distributed. Video recording is expected to be filmed on phone, on a fixed tripod, the recording, where possible, should include the participant playing the game, a side by side recording of the computer screen would start concurrently for retrospective analysis.

   2. Written notes - we expect to obtain written notes written by our evaluation team member generated through observing participants play the game. These notes will document observer's insights of participant's interaction with the game.

   3. Interview answers - we expect to have obtained interview answers from participants, these might be in the form of a physical note, digital word document or other medium.

   All data collected from the evaluation process will be converted to digial format wherever possible, they would be stored collectively on a UniMelb student google drive, consent to these data collection from the participants would be stored side by side to the obtained material. These material could include but not limited to txt, word document, voice recording, video recording, scanned copy of written notes etc.


**7. Analysis and Evaluation:**
   
   The game will be evaluated with criteria listed in **Evaluation Criteria**. Participants will be asked questioned regarding aforementioned elements.
   
   Coding would be applied to all textual response, common themes would be identified where significant number of participants reported issues. Problems would be aggregated into archtypes and subtypes which can faciliate designing solutions that can address these issues systematically rather than on a case by case basis. 
   
   Analysis is expected to be derived from quotes generated from participants' mention of issues regarding any elements in the game. For example, gameplay is overtly repetitve, where participants raise issues like not enough diverse character movements, not enough custom abilities etc, the development team would attempt to address the issue through first looking at how much space is avaliable for changes and reworks. Most importantly, trade offs should be carefully considered between participant suggestions that would alter game mechanics flavour beyond what was initially intended and the benefits implementing these changes would bring to players to better enjoy the game. Given the feedback, we aim to find the core issue that is causing pains to the game experience, and seeks to address them.

   Basic elements like replayability, engagement, interesting design etc will be rated through a Likert Scale from strongly disagree to strongly agree. The interview will ask participant whether they agree above elements are properly implemented in the game. Where a certain element consistently rated negatively among the participants, it would be subject to reworks. Direction of rework depends on participant feedback provided from open-ended questions in the interview.

**8. Post-Evaluation Activities:**
   - Redesign elements that are raised as significant difficulties or impediments to a fluid gaming experience. 
   - Trial implementation of altered designs, interal tests conducted by other team members, compare difference between two versions, if the benefit could be ascertained, the changes would be accepted and moved into final product.
   - Analyse which deisgn decisions contributed to creating the issue, changes to the design process could be implemented to avoid future repeition of similar problems.

**9. Timelines:**

Stage 1. Recuritment. Finish recruiting participants and filter out participants not suitable for this evaluation process. - Due 9th Oct.

Stage 2. Scheduling Sessions, scheduling available time with participant to begin playthrough. 

Stage 3. Began evaluation. Starting Structured Observation &  interviews with participants, gather participant consent when they accept scheduled time.

Stage 4. Filter results. Aggregate all results and feedbacks gathered from evaluation, began coding and analysing feedbacks.

Stage 5. Conclude evaluation process. Due 15th Oct. 

Stage 6. Compling report. List out areas subject for changes and fixes. Compiling Evaluation report. Due 15th Oct.

Stage 7. Implementing changes. Implement changes and play test changes, accept or reject changes depend on result. Due 20th Oct.


## Evaluation Report

TODO (due milestone 3) - see specification for details

## Shaders and Special Effects

TODO (due milestone 3) - see specification for details

## Summary of Contributions

TODO (due milestone 3) - see specification for details

## References and External Resources

TODO (to be continuously updated) - see specification for details
[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/0n2F_Zha)