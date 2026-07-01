# SonarQube Cloud - Analyse qualite PartFinder

Ce document explique comment lancer une analyse SonarQube Cloud pour obtenir des indicateurs reels sur la qualite du code du projet PartFinder.

## Objectif

L'analyse SonarQube Cloud permet d'obtenir des indicateurs sur :

- les bugs potentiels ;
- les vulnerabilites ;
- les code smells ;
- la duplication ;
- la maintenabilite ;
- la dette technique ;
- la couverture de tests si elle est detectee.

Ces resultats peuvent etre utilises dans le dossier professionnel, dans la partie Gouvernance, Qualite et Amelioration continue du SI.

## Configuration SonarQube Cloud

1. Aller sur https://sonarcloud.io
2. Se connecter avec le compte GitHub.
3. Importer le depot GitHub `elamrani7/CapLed`.
4. Verifier les valeurs suivantes dans SonarQube Cloud :
   - Organization key : `elamrani7`
   - Project key : `elamrani7_CapLed`

Si SonarQube Cloud donne d'autres valeurs, modifier le fichier :

```text
.github/workflows/sonarqube-cloud.yml
```

et remplacer :

```text
SONAR_PROJECT_KEY: elamrani7_CapLed
SONAR_ORGANIZATION: elamrani7
```

## Secret GitHub obligatoire

Dans GitHub :

1. Aller dans le depot `CapLed`.
2. Ouvrir `Settings`.
3. Aller dans `Secrets and variables` puis `Actions`.
4. Ajouter un nouveau secret :

```text
SONAR_TOKEN
```

La valeur doit etre le token genere dans SonarQube Cloud.

Ne jamais mettre ce token dans le code source.

## Lancer l'analyse

1. Aller dans GitHub Actions.
2. Selectionner le workflow `Code Quality - SonarQube Cloud`.
3. Cliquer sur `Run workflow`.
4. Choisir la branche `stable-version`.
5. Lancer le workflow.

Si l'analyse passe en vert, ouvrir SonarQube Cloud pour consulter le tableau de bord qualite.

## Figures conseillees pour le rapport

Figure 1 :

```text
Dashboard SonarQube Cloud du projet PartFinder
```

Cette capture doit montrer le Quality Gate, les bugs, les vulnerabilites, les code smells, les duplications et la dette technique.

Figure 2 :

```text
Workflow GitHub Actions Code Quality - SonarQube Cloud reussi
```

Cette capture montre que l'analyse qualite est integree au processus DevOps.

## Formulation possible pour le rapport

Afin d'evaluer la qualite du code de maniere plus objective, une analyse statique a ete mise en place avec SonarQube Cloud. Cet outil permet d'identifier les bugs potentiels, les vulnerabilites, les duplications, les problemes de maintenabilite et la dette technique. Cette analyse complete les tests automatises et permet de suivre l'amelioration continue du systeme d'information.
