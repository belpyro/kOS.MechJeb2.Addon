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

### kOS addon access pattern

The addon is registered in kOS via the `[KOSAddon("MJ")]` attribute, so the correct entry point is:

```ks
set mj to addons:mj.
```

From there you can access the MechJeb core wrapper and its sub-wrappers:

```ks
set mj   to addons:mj.
set core to mj:core.

// Vessel state wrapper
set v    to core:vessel.

// Info items wrapper
set info to core:info.

// Ascent autopilot wrapper
set asc  to core:ascent.
```

---

## 🔗 API Overview

### Top-level addon suffixes (`ADDONS:MJ`)

| Suffix      | Type               | Description                                  |
|-------------|--------------------|----------------------------------------------|
| `CORE`      | MechJebCoreWrapper | Entry point to all MechJeb-related wrappers  |
| `AVAILABLE` | Boolean            | True if MechJeb is available for this vessel |
| `VERSION`   | VersionInfo        | Actual plugin version                        |

```ks
set mj to addons:mj.

if not mj:available {
    print "MechJeb is not installed for this vessel.".
}

set core to mj:core.
```

---

### Core wrapper (`ADDONS:MJ:CORE`)

| Suffix   | Type                   | Description                                     |
|----------|------------------------|-------------------------------------------------|
| `VESSEL` | VesselStateWrapper     | Vessel flight state (altitude, speed, Q, AoA…) |
| `INFO`   | MechJebInfoItemsWrapper| MechJeb info values (ΔV, TWR, orbit, target…)  |
| `ASCENT` | MechJebAscentWrapper   | Ascent autopilot control                        |
| `RUNNING`| Boolean                | True if MechJeb core is initialized             |

```ks
set core to addons:mj:core.

print core:running.
set v    to core:vessel.
set info to core:info.
set asc  to core:ascent.
```

---

### Vessel state wrapper (`ADDONS:MJ:CORE:VESSEL`)

This wrapper exposes real-time vessel state (numbers mostly taken from MechJeb's internal `VesselState`).  
All suffix names are case-insensitive in kOS; here they are shown in UPPERCASE as they are registered.

#### Time & gravity

| Suffix  | Alias | Description                                 |
|---------|-------|---------------------------------------------|
| `TIME`  | `T`   | Universal time in seconds                   |
| `LOCALG`| `G`   | Local gravitational acceleration (m/s²)     |

#### Velocities

| Suffix                   | Alias     | Description                                                   |
|--------------------------|-----------|---------------------------------------------------------------|
| `SPEEDORBITAL`           | `ORBVEL`  | Orbital speed relative to the reference body (m/s)           |
| `SPEEDSURFACE`           | `SURFVEL` | Surface speed relative to the rotating body (m/s)            |
| `SPEEDVERTICAL`          | `VERTVEL` | Vertical component of surface velocity (m/s)                 |
| `SPEEDSURFACEHORIZONTAL` | `SURFHVEL`| Horizontal component of surface velocity (m/s)               |
| `SPEEDORBITHORIZONTAL`   | `ORBHVEL` | Horizontal component of orbital velocity (m/s)               |

#### Attitude

| Suffix          | Alias   | Description                                     |
|-----------------|---------|-------------------------------------------------|
| `VESSELHEADING` | `HDG`   | Vessel heading in degrees (0 = North)          |
| `VESSELPITCH`   | `PITCH` | Vessel pitch in degrees (0 = horizon, + up)    |
| `VESSELROLL`    | `ROLL`  | Vessel roll in degrees                         |

#### Altitudes

| Suffix              | Alias    | Description                                             |
|---------------------|----------|---------------------------------------------------------|
| `ALTITUDEASL`       | `ALTASL` | Altitude above sea level (m)                           |
| `ALTITUDETRUE`      | `ALTTRUE`| True altitude above terrain (m)                        |
| `SURFACEALTITUDEASL`| `SURFALT`| Surface altitude above sea level under the vessel (m)  |

#### Orbit

| Suffix                     | Alias   | Description                                  |
|----------------------------|---------|----------------------------------------------|
| `ORBITAPA`                 | `APA`   | Apoapsis altitude above sea level (m)        |
| `ORBITPEA`                 | `PEA`   | Periapsis altitude above sea level (m)       |
| `ORBITPERIOD`              | `ORBITPER` | Orbital period (s)                        |
| `ORBITTIMETOAP`            | `TTOAP` | Time to apoapsis (s)                         |
| `ORBITTIMETOPE`            | `TTOPE` | Time to periapsis (s)                        |
| `ORBITLAN`                 | `LAN`   | Longitude of ascending node (deg)            |
| `ORBITARGUMENTOFPERIAPSIS` | `ARGPE` | Argument of periapsis (deg)                  |
| `ORBITINCLINATION`         | `INCL`  | Orbital inclination (deg)                    |
| `ORBITECCENTRICITY`        | `ECC`   | Orbital eccentricity                         |
| `ORBITSEMIAXIS`            | `SMA`   | Orbital semi-major axis (m)                  |

#### Surface position

| Suffix              | Alias    | Description                                         |
|---------------------|----------|-----------------------------------------------------|
| `CELESTIALLONGITUDE`| `CESTLON`| Sub-vessel longitude on the celestial body (deg)    |
| `LATITUDE`          | `LAT`    | Vessel latitude (deg)                               |
| `LONGITUDE`         | `LON`    | Vessel longitude (deg)                              |

#### Aero angles

| Suffix              | Alias    | Description                                                              |
|---------------------|----------|--------------------------------------------------------------------------|
| `AOA`               | _(none)_ | Angle of attack relative to airflow (deg)                                |
| `AOS`               | _(none)_ | Angle of sideslip relative to airflow (deg)                              |
| `DISPLACEMENTANGLE` | `DISPANG`| Angle between velocity vector and reference orientation (deg)            |

#### Aero / speed / drag

| Suffix         | Alias | Description                                          |
|----------------|-------|------------------------------------------------------|
| `MACH`         | `MACH`| Mach number (speed relative to speed of sound)      |
| `SPEEDOFSOUND` | `SOS` | Local speed of sound (m/s)                          |
| `DRAGCOEF`     | `CD`  | Effective drag coefficient                          |

#### Atmosphere / pressure

| Suffix                   | Alias  | Description                                              |
|--------------------------|--------|----------------------------------------------------------|
| `ATMOSPHERICDENSITYGRAMS`| `RHO`  | Atmospheric density (g/m³)                               |
| `MAXDYNAMICPRESSURE`     | `QMAX` | Maximum dynamic pressure experienced this flight (Pa)    |
| `DYNAMICPRESSURE`        | `Q`    | Current dynamic pressure (Pa)                            |

#### Intake air

| Suffix              | Alias       | Description                                                  |
|---------------------|-------------|--------------------------------------------------------------|
| `INTAKEAIR`         | `INTAKE`    | Instantaneous intake air resource amount                     |
| `INTAKEAIRALLINTAKES`| `INTAKEALL`| Total intake air supply from all intakes                     |
| `INTAKEAIRNEEDED`   | `INTAKENEED`| Intake air needed by engines at current throttle             |
| `INTAKEAIRATMAX`    | `INTAKEMAX` | Intake air supply at maximum speed/flow                      |

#### Misc orbital / aerothermal

| Suffix                        | Alias    | Description                                                             |
|-------------------------------|----------|-------------------------------------------------------------------------|
| `ANGLETOPROGRADE`            | `ANGPRO` | Angle between vessel forward and velocity prograde vector (deg)        |
| `FREEMOLECULARAEROTHERMALFLUX`| `FMFLUX`| Estimated free molecular aerothermal flux (W/m²)                        |

#### Net forces

| Suffix     | Alias | Description                         |
|------------|-------|-------------------------------------|
| `PUREDRAG` | `DRAG`| Net drag force magnitude (kN)       |
| `PURELIFT` | `LIFT`| Net lift force magnitude (kN)       |

#### Example

```ks
set v to addons:mj:core:vessel.

print "Surface speed: " + v:SPEEDSURFACE.
print "Altitude ASL:  " + v:ALTITUDEASL.
print "Q:             " + v:DYNAMICPRESSURE.
print "Mach:          " + v:MACH.
print "AoA:           " + v:AOA.
```
---

### Info items wrapper (`ADDONS:MJ:CORE:INFO`)

This wrapper exposes MechJeb “Info Items” (methods marked with `ValueInfoItem`) as kOS suffixes.  
All suffix names are case-insensitive in kOS; here they are shown in UPPERCASE as they are registered.

#### Maneuver / node

| Suffix                     | Alias     | Description                         |
|----------------------------|-----------|-------------------------------------|
| `NEXTMANEUVERNODEBURNTIME` | `BURN`    | Burn time for next maneuver node    |
| `TIMETOMANEUVERNODE`       | `NODEETA` | Time to next maneuver node          |
| `NEXTMANEUVERNODEDELTAV`   | `NODEDV`  | ΔV of the next maneuver node        |

#### TWR / thrust / acceleration

| Suffix          | Alias    | Description                           |
|-----------------|----------|---------------------------------------|
| `SURFACETWR`    | `STWR`   | Surface TWR                           |
| `LOCALTWR`      | `LTWR`   | Local TWR                             |
| `THROTTLETWR`   | `TTWR`   | TWR at current throttle               |
| `CURRENTACC`    | `ACC`    | Current acceleration                  |
| `CURRENTTHRUST` | `THRUST` | Current thrust (kN)                   |
| `MAXTHRUST`     | `MAXTH`  | Maximum possible thrust               |
| `MINTHRUST`     | `MINTH`  | Minimum engine thrust                 |
| `MAXACC`        | `MAXACC` | Maximum acceleration                  |
| `MINACC`        | `MINACC` | Minimum acceleration                  |
| `ACCELERATION`  | `A`      | Net acceleration                      |

#### Atmosphere / pressure / drag

| Suffix           | Alias   | Description                         |
|------------------|---------|-------------------------------------|
| `ATMPRESSUREKPA` | `P_KPA` | Atmospheric pressure (kPa)         |
| `ATMPRESSURE`    | `P`     | Atmospheric pressure (normalized?) |
| `ATMDESITYDRAG`  | `DRAG`  | Drag force estimation              |
| `DRAGCOEF`       | `CD`    | Aerodynamic drag coefficient       |

#### Coordinates / position

| Suffix             | Alias   | Description                     |
|--------------------|---------|---------------------------------|
| `COORDINATESTRING` | `COORD` | Formatted latitude/longitude    |

#### Orbit basics

| Suffix                   | Alias      | Description                            |
|--------------------------|------------|----------------------------------------|
| `MEANANOMALY`            | `MA`       | Mean anomaly                           |
| `CURRENTORBITSUMMARY`    | `ORBSUM`   | Current orbit summary                  |
| `TARGETORBITSUMMARY`     | `TORBSUM`  | Target orbit summary                   |
| `CURRENTORBITSUMMARYINC` | `ORBSUMINC`| Orbit summary incl. inclination        |
| `TARGETORBITSUMMARYINC`  | `TORBSUMINC`| Target orbit summary incl. incl.     |
| `ORBITALENERGY`          | `ENERGY`   | Specific orbital energy                |
| `POTENTIALENERGY`        | `POT`      | Potential orbital energy               |
| `KINETICENERGY`          | `KIN`      | Kinetic energy                         |
| `TIMETOIMPACT`           | `IMPACT`   | Time until surface impact              |
| `SUICIDEBURNCOUNTDOWN`   | `SBC`      | Time until suicide burn                |
| `TIMETOSOITRANSITION`    | `SOIETA`   | Time to SOI transition                 |
| `SURFACEGRAVITY`         | `g`        | Surface gravity                        |
| `ESCAPEVELOCITY`         | `VESC`     | Escape velocity                        |
| `CIRCULARORBITSPEED`     | `VCIRC`    | Circular orbit velocity                |

#### RCS

| Suffix              | Alias    | Description                      |
|---------------------|----------|----------------------------------|
| `RCSTHRUST`         | `RCSF`   | RCS thrust                       |
| `RCSTRANSLATIONEFF` | `RCSEFF` | RCS translation efficiency       |
| `RCSDELTAVVAC`      | `RCSDV`  | RCS ΔV in vacuum                 |

#### Angular velocity

| Suffix            | Alias    | Description         |
|-------------------|----------|---------------------|
| `ANGULARVELOCITY` | `ANGVEL` | Angular velocity    |

#### Vessel basics

| Suffix            | Alias    | Description                          |
|-------------------|----------|--------------------------------------|
| `VESSELNAME`      | `NAME`   | Vessel name                          |
| `VESSELTYPE`      | `TYPE`   | Vessel type                          |
| `VESSELMASS`      | `MASS`   | Total vessel mass                    |
| `MAXVESSELMASS`   | `MASSMAX`| Maximum vessel mass                  |
| `DRYMASS`         | `DRYM`   | Dry mass                             |
| `LFO_MASS`        | `LFO`    | Liquid fuel and oxidizer mass        |
| `MONOPROP_MASS`   | `MP`     | MonoPropellant mass                  |
| `ELECTRICCHARGE`  | `EC`     | Electric charge                      |
| `PARTCOUNT`       | `PC`     | Part count                           |
| `MAXPARTCOUNT`    | `PCMAX`  | Maximum part count                   |
| `PARTCOUNTANDMAX` | `PCM`    | Part count with max info             |
| `STRUTCOUNT`      | `STRUTS` | Strut count                          |
| `FUELLINESCOUNT`  | `LINES`  | Fuel lines count                     |
| `VESSELCOST`      | `COST`   | Vessel cost                          |
| `CREWCOUNT`       | `CREW`   | Crew count                           |
| `CREWCAPACITY`    | `CAP`    | Crew capacity                        |

#### Target distance / relative motion

| Suffix                       | Alias   | Description                           |
|------------------------------|---------|---------------------------------------|
| `TARGETDISTANCE`             | `TDIST` | Distance to target                    |
| `HEADINGTOTARGET`            | `THDG`  | Heading to target                     |
| `TARGETRELV`                 | `TRELV` | Relative velocity to target           |
| `TARGETTTCLOSEAPP`           | `TCA`   | Time to closest approach              |
| `TARGETCLOSEAPPDIST`         | `CADIST`| Closest approach distance             |
| `TARGETCLOSEAPPRELV`         | `CAREL` | Relative velocity at closest approach |

#### Biomes

| Suffix            | Alias      | Description           |
|-------------------|------------|-----------------------|
| `CURRENTRAWBIOME` | `RAWBIOME` | Raw biome identifier  |
| `CURRENTBIOME`    | `BIOME`    | Display biome name    |

#### Stage ΔV / burn time

| Suffix              | Alias    | Description                                |
|---------------------|----------|--------------------------------------------|
| `STAGEDELTAVVAC`    | `SDV`    | Stage ΔV in vacuum                         |
| `STAGEDELTAVATM`    | `SDVATM` | Stage ΔV in atmosphere                     |
| `STAGEDELTAVATMVAC` | `SDVSTR` | Combined/Formatted stage ΔV atm/vac        |
| `STAGETIMEFULL`     | `TBURNF` | Stage burn time at full throttle           |
| `STAGETIMECURRENT`  | `TBURNC` | Stage burn time at current throttle        |
| `STAGETIMEHOVER`    | `THOVER` | Stage hover time                           |
| `TOTALDVVAC`        | `TDV`    | Total vessel ΔV in vacuum                  |
| `TOTALDVATM`        | `TDVATM` | Total vessel ΔV in atmosphere              |
| `TOTALDVATMVAC`     | `TDVSTR` | Combined/Formatted total ΔV atm/vac        |

#### Example

```ks
set info to addons:mj:core:info.

print "Surface TWR:    " + info:SURFACETWR.
print "Total DV (vac): " + info:TOTALDVVAC.
print "Stage DV (vac): " + info:STAGEDELTAVVAC.
print "Target dist:    " + info:TARGETDISTANCE.
print "Biome:          " + info:CURRENTBIOME.
```

---

### Ascent autopilot wrapper [WIP] (`ADDONS:MJ:CORE:ASCENT`)

Use MechJeb's ascent guidance from kOS.

Common operations:
- `IsEnabled`

Example:

```ks
set asc to addons:mj:core:ascent.

if not asc:isEnabled {
    print "Engaging MechJeb ascent...".
}
```

---
