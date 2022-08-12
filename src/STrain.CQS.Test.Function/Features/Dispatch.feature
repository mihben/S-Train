Feature: [DSP] - Dispatch

Responsible for perform the proper performer according to incoming request. Exact one performer allowed for each requests.

	@issue-6
	@unit
	Scenario: [UNIT][DSP-001] - Dispatch command
		Given Performer registrered for command
		When Dispatching command
		Then Command performer should be performed

	@issue-6
	@unit
	Scenario: [UNIT][DSP-002] - Performer not exists for command
		When Dispatching command
		Then NotImplementedException should be thrown
		
	@issue-6
	@unit
	Scenario: [UNIT][DSP-003] - Dispatch query
		Given Performer registrered for query
		When Dispatching query
		Then Query performer should be performed

	@issue-6
	@unit
	Scenario: [UNIT][DSP-004] - Performer not exists for query
		When Dispatching query
		Then NotImplementedException should be thrown