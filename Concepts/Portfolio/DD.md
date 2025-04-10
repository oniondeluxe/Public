#Due diligence

I have on several occasions been involved in due diligence (DD, from now on), in the context of company acquisitions. There, I vouched for the technical part of the investigations, i.e. the one that focuses on the engineering department. Sometimes undercover.  
Perhaps the main thing you want to know in such investigations is the answer to the questions:  
What are these things good for? Can they handle the pressure? If so, how expensive is it to keep them up to scratch? And how well equipped is the organization to manage it securely and effectively?  
The answers are hidden in the big and the small. A perfect and flawless technology platform won't go far if employees don't know how to work with it in a sensible way. And the flip side is also true: you can have the best processes and culture in the world, but you'll be stuck in quicksand if the backing technology it is a dinosaur.  
You also want to be able to identify red flags. You can illustrate that part in the analogy of Swiss cheese; when you put the slices on top of each other, you don't want too many holes in the cheese to gaze through all the way down to the slice of bread.
What I will cover in this article is the approach to conducting a DD, and I will conclude with an example of a real-life case; the impending acquisition of a telecom company in the southern hemisphere.  
As a disclaimer, it must incidentally be mentioned that it is of course a challenge to be able to capture all aspects of the condition of a development department in such a relatively limited time and resource space. A significant part of the assessment is based on long experience of working in the industry, at different levels, and with different types of interactions between roles and processes.  

## Structure
The survey can be divided into three parts:  
The purely static inventory of the technical platform, i.e. the code base and surrounding technical ecosystems and processes.
Assessments that highlight how people work, internally within the engineering guild.
Studies that illustrate how the collaboration between the different departments works, starting from the development department.
What I don't include are topics that don't have immediate touch points with the engineering department, such as sales processes, accounting or HR. The DD teams I have worked in have had specific expertise for this.
Static inventory
If you buy a used car, you want to know as much as possible about its condition, so that you don't have to change the engine, for example, or have other expensive repairs carried out just a few days after purchase.
The purpose of the static inventory is to inspect the condition of the technical platform. I then look at these areas:
- What does the code base look like?
- What is the state of the surrounding support processes?
What I have investigated is primarily traditional three-tier applications in the enterprise segment. There are of course other incarnations of software, such as flight simulators, games or industrial automation software. This may differ somewhat, and is beyond the scope of this article.
The results of these studies are then presented in a report. For quantification, a five-point scale has been used. Some tidbit result points, are as follows:
1. The degree of technical excellence according to SOLID [1 – 5] (conformity with industry standard)
2. How easy to read and how easy to navigate is the code base [1 – 5] (start-up time for new team members)
3. How easy is it to push the release button [1 – 5] (how much manual bottlenecks are there and how well agile/automated methods can be supported)
4.How well do the processes support immediate incident handling [1 – 5] (how do you handle hotfixes so that you don't have immediate or future crashes and/or data loss)
In parallel, a template I often use is to simulate some developer use cases, i.e. common scenarios in the daily work of the engineering department. For example: describe the procedure, all the way, to add a new column to a business object.
Finally, get a grasp of what possibilities could exist for the existing building blocks to be integrated and/or reused with the existing product range of our organization (the one acquiring the company subject to the DD).
As part of the strict financial valuation of the company, this section also includes an inventory of tools, licenses, subscriptions, etc.
Internal processes, part 1
The core of this is about how the team(s) work, and where relevant, how the teams interact with each other. What you want to know is:

- Are we delivering? (there are actually examples of companies that have not released anything at all, for 18 months)
- Are we delivering as expected? I.e. are customers getting what they want and at a reasonable quality?
- Are we delivering at a reasonable cost? How labor intensive is the release of new features?
- Are we getting any leverage effects, and if so, how are they quantified? I.e., is it getting easier and easier to release new functionality, or will we eventually succumb to old sins and mishaps?
- What percentage is waste or duplication? Relevant especially when you have multiple teams.
The first stage of this investigation is to read the policy documents and process descriptions. Taking into account the points above, an interview template is then adapted with the aim of finding evidence of how the points are met.
Once this is done, initial interviews are conducted with team leaders, and other key people with direct contact with the daily processes, such as test managers.
Security
What processes are in place? How often is live testing done?
Interfaces
The development department is a delivery engine in the company. But how well does it fit in with the rest of the business? How expensive and painful is it for other parties in the organization to “conduct business” with the development department? Are things being done in the right places and at the right level?
In addition to the material from the policy documents, interviews are conducted with people in operational management positions in:
- Product managers
- Support
- Programme manager (explain)
- Marketing and/or sales
Examples of interview topics include 
Incidents
How quickly are incidents handled, in terms of response time, handshakes, forecasts and actions? What is the business continuity plan? Are interruptions handled according to a routine, or is an adhoc culture adopted? What is the follow-up for identified defects, and how is this hand shaken between product, support and development? 
Upgrades
How easy do customers make their upgrades? How much engineering time needs to be spent on customized, customer-targeted fixes? What is the strategy for customization, in the context of upgrades?
The quantification is done with respect to how other departments perceive the engineering department. The results are summarized in a graded scale, according to the following guidelines:
- Quality/Reliability (predictability) [1 – 5]
- Mobility/Willingness to serve [1 – 5]
- Parallel cost (what it costs in resources other than dev) [1 – 5]
Note that quality in this context does not mean the percentage of defects in the software itself, but how professionally and smoothly the processes are carried out.
Internal processes, part 2
Once you have captured the overall situation in the production tier, and how well everything fits in with the rest of the business, investigations are done at a more senior level. This involves interviews with the head of the development department, the CTO and/or CIO, etc.
- How consistent is the internal view of the business with the view revealed by the DD survey so far? How is the awareness about flaws and deviations?
- What plans and strategies are in place to improve defects in technology and processes?
- What observability is there for cost control, and how do those processes work?
- How well equipped is the development department to deal with competitive pressures? Both from the point of view of agility and resilience, and in terms of recruitment.
- How is the skills management? Is there a reasonable spread and good risk management?
- Which elements of the technology platform could pose potential problems, should more modern technologies gain a foothold in the industry?
Pure product strategy decisions and processes that only indirectly affect other departments tend to be left out of this part of the DD. For example, what is the attract and repel strategy, i.e. which customers do we want to attract and which do we want to repel?
Approach
In order not to disclose sensitive information, some details have been omitted. However, this does not affect the overall meaning and key points.
The team
The DD team consists of 3–5 representatives from the parent company, with different competences, who work operationally, in a similar way, during the period of investigation. These are functions such as sales, accounting and HR.
Schedule
The schedule is intended to represent a sequence of work shifts. For illustrative purposes, it can be assumed to run for a number of days 1–5, but the schedule can also be compressed/expanded depending on the size of the project.
After daily completed interview sessions, a review was done together with the other participants in the DD group.
The results were summarized in reports, but also through 2–3 verbal reconciliations with key members of the senior management staff who were also present on certain days.
Day/Phase 1
Presentation of the DD team. Readings of the main internal documents.
Product presentation and demo, usually from the sales department or presales.
Day/Phase 2
Internal processes, part 1
Day/Phase 3
Static inventory
Day/Phase 4
Internal processes, part 2
Day/Phase 5
Loose threads
Spare time
Summary
In this case, the DD was quite successful. After some time, the company in question was incorporated with the rest of our organization.
