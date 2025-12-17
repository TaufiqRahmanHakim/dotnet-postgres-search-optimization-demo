# API Performance Optimization: From 20s to 40ms

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![k6](https://img.shields.io/badge/k6-7D64FF?style=for-the-badge&logo=k6&logoColor=white)

## Executive Summary

This demonstrates a **Performance Engineering** case study. The goal was to optimize a search API handling **500,000 records** that was suffering from critical latency issues under load.

Through database profiling (Explain Analyze) and load testing (k6), I identified the bottleneck and implemented **Functional B-Tree Indexing** strategies.

**The Result:**
* **Latency (P95):** Reduced from **20.8s** to **40ms** (**500x Faster**).
* **Throughput:** Increased from crashing at 10 users to handling **50-100 concurrent users** with 0% error rate.
* **Efficiency:** Eliminated Full Table Scans in favor of Index Seeks.

---

## The Problem (Before Optimization)

The initial implementation used a naive approach for searching text data (`ILike` with `%wildcard%`).
* **Dataset:** 500,000 Customer Records.
* **Query Logic:** `WHERE name ILIKE '%search%'`
* **Bottleneck:** PostgreSQL could not utilize standard indexes due to the leading wildcard and case-insensitive query, resulting in a **Parallel Sequential Scan (Full Table Scan)** for every request.

### Stress Test Benchmark (Baseline)
Running `k6` with 50 Virtual Users resulted in system failure:
* X **Avg Latency:** ~20 seconds (Timed out)
* X **Error Rate:** 12% - 74% (Server Overload)
* X **Experience:** Unusable.

## Before Optimization k6 Result vas 50 & 60 sec
<img width="713" height="423" alt="image" src="https://github.com/user-attachments/assets/119c4841-16ca-457f-86d1-10e4a7d0f28a" />

## After Optimization k6 Result vas 100 & 60 sec
<img width="705" height="437" alt="image" src="https://github.com/user-attachments/assets/77ef3536-266e-4351-8c90-f886c6f2131a" />


---

## 🛠️ The Solution (Engineering Fix)

To solve this, I moved from application-level logic to database-level optimization using **Functional Indexing**.

### 1. Database Optimization (PostgreSQL)
I created a specific index to handle case-insensitive prefix searches. I utilized the `text_pattern_ops` operator class to ensure the B-Tree index supports efficient `LIKE 'prefix%'` queries regardless of locale settings.

```sql
-- The Magic Index
CREATE INDEX idx_customers_firstname_lower 
ON "Customers" (lower("FirstName") text_pattern_ops);

```

##  How to Run

1. **Initialize Database:**
   Run `dotnet ef database update` or `database-update` to create the schema.
   
2. **Seed Data:**
   Run the Console Application project to automatically populate the database with dummy data.

3. **Start Server:**
   Run the API project (`dotnet run`).

4. **Execute Load Test:**
   ```bash
   k6 run --out web-dashboard load_test.js
