# PTT-Archiv Query Expansion

## Description
Search queries are often dependent on a very precise selection of keywords; if Bern is also mentioned as Berne or Berna in documents, it is quickly sorted out as a hit. To actively circumvent this, there is the so-called query expansion, which includes other forms of the same concept in the search query.
The goal of our project is the implementation of query expansion based on geo and personal data records (focus on Switzerland) for the PTT archive.


## Stakeholders and Roles
- Nicolas Kessler: Customer and Product Owner
- BFH: Partner and Employer
- Benjamin Fehrensen: Project supervisor
- Michael Luggen: Technical supervisor
- Chun Fatt Wong, Nadia Suter, Ann-Sophie Junele: Developer Team
- Chun Fatt Wong: Scrum Master

## Get started

### Run backend locally
1. Follow the ConsoleApp Documentation to fill the database: [PTT-Archiv DB-Updater](https://gitlab.ti.bfh.ch/Project_1_Group_17_BTI3031p-21-22/ptt-archiv_query_expansion/-/blob/master/DB_Console_App/documentation.md)
2. Requirements (there are a lot of tutorials online if they are not already installed):
- Maven
- Java 11 (jdk11)
3. Open a terminal in the backend project folder (ptt-archiv_query_expansion/backend)
4. Run `mvn install`
5. Run `mvn spring-boot:run`
6. The backend should now run on http://localhost:8080

### Run frontend locally
1. Requirements:
- node.js
- npm
2. Open a terminal in the frontend project folder (ptt-archiv_query_expansion/frontend)
3. Run `npm install -g @angular/cli`
4. Run `ng serve`
5. The frontend should now run on http://localhost:4200

### Generate javadoc (backend documentation)
1. Open a terminal in the backend project folder (ptt-archiv_query_expansion/backend)
2. Run `mvn javadoc:javadoc`
3. The javadoc should now be in the following folder: /ptt-archiv_query_expansion/backend/target/site/apidocs/index.html

