# 🚀 kOS.MechJeb.Addon

**Integration layer between kOS and MechJeb2**

This addon provides a bridge between **kOS** and **MechJeb2**, allowing you to:

- read internal MechJeb data directly from kOS scripts;
- access MechJeb autopilot modules as kOS objects;
- use the same values that appear in MechJeb windows (ΔV, Orbit Info, Vessel Info, Targeting, etc.).

---

## ✨ Features (WIP)

- 🔍 **Access to MechJeb Info Items** (via the `ValueInfoItem` attribute):
    - per-stage and total ΔV;
    - TWR, thrust, acceleration;
    - orbital parameters (energy, anomalies, apsides, period, ETA);
    - target parameters (distance, relative velocity, phase angle, closest approach);
    - RCS thrust & efficiency;
    - vessel mass, resources, cost, crew;
    - biome, coordinates, atmosphere data, etc.

- 🧠 **Wrappers for MechJeb modules**:
    - Ascent Autopilot
    - Attitude Controller
    - Target Controller
    - Maneuver Node Executor
    - Landing Autopilot
    - Stage Stats
    - Many more…

- ⚙️ **High-performance reflection layer**:
    - one-time type analysis;
    - compiled delegates using Expressions;
    - cached method access;
    - no heavy `MethodInfo.Invoke` in runtime.

- 🧩 **Clean kOS API**:
    - user-friendly suffixes;
    - short aliases (`Q`, `TWR`, `APA`, `PEA`, etc.);
    - suffix descriptions for in-kOS help.

---

## 📦 Installation

1. Install **kOS**.
2. Install **MechJeb2**.
3. Copy the addon folder into your `GameData` directory

### Requirements

- **KSP 1.12.x**
- **kOS 1.3.0+**
- **MechJeb2 (latest dev build recommended)**

---

## 🕹 Using the Addon in kOS

### Accessing the addon

```ks
set mj to addons:mj:core.
```

TODO: Syntax definition