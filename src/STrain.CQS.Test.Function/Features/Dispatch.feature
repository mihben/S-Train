Feature: Dispatch

Responsible for perform the proper performer according to incoming request. Exact one performer allowed for each requests.

	@issue-6
	@unit
	Scenario: Dispatch command
		Given Performer registrered for command
		When Dispatching command
		Then Command performer should be performed

	@issue-6
	@unit
	Scenario: Performer not exists for command
		When Dispatching command
		Then NotImplementedException should be thrown

	@issue-6
	@unit
	Scenario: Multiple performer registered for command
		Given Multiple performer registered for command
		When Dispatching command
		Then InvalidOperationException should be thrown
		
	@issue-6
	@unit
	Scenario: Dispatch query
		Given Performer registrered for query
		When Dispatching query
		Then Query performer should be performed

	@issue-6
	@unit
	Scenario: Performer not exists for query
		When Dispatching query
		Then NotImplementedException should be thrown

	@issue-6
	@unit
	Scenario: Multiple performer registered for query
		Given Multiple performer registered for query
		When Dispatching query
		Then InvalidOperationException should be thrown