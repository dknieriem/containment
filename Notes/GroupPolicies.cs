GroupPolicies.cs

Resource Management:

Policy Set: Policy[]

struct PolicyEffect{
	string statAffected;
	int percentDelta;
	int absoluteDelta;
}

Policy: string Name, PolicyEffect

-Food Management:
	0. Subsistence 
		-30% food consumption 
		-50% to all physical stats
		-25% to all mental stats
	1. Tightening the Belt 
		-15% food consumption
		-25% to all physical stats
		-10% to all mental stats
	2. Default
	3. Land of Plenty
		+15% food consumption
		+10% to all physical stats
		+5% to all mental stats

-Ammo Management
	0. Save the last bullet for yourself
		-100% ammo usage
		-100% ranged weapon effect
		-50% mental stability
	1. Last Resort
		-50% ammo usage
		-75% ranged weapon effect
		-30% mental stability
	2. Conservative
		-25% ammo usage
		-50% ranged weapon effect
		-5% mental stability
	3. Default
	4. Liberal
		+25% ammo usage
		+15% ranged weapon effect
		+1% mental stability

-Repair Management ??
	0. We just can't afford it
		-100% materials usage
		-100% repair speed
	1. 