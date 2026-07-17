# Akasha

**Akasha** is a BepInEx plugin for **Nuclear Option** that collects real-time game statistics and forwards them to Apache Kafka. The companion **Irminsul** worker consumes the data and stores it in PostgreSQL for analysis and visualization.

## Status (WIP)
- [ ] Writing .NET worker service to save data to DB.
- [ ] Writing ASP.NET webAPI for future web site.

## Architecture

Nuclear Option Server (with Akasha) 
       ↓ 
Kafka (topic: match-results) 
       ↓ 
Irminsul (.NET Worker) 
       ↓ 
PostgreSQL