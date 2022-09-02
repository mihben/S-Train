Feature: [DSP] - Dispatch

Responsible for perform the proper performer according to incoming request. Exact one performer allowed for each requests.

	Rule: [DSP/DSP] - Dispatching requests

		@issue-6
		@unit
		Scenario: [UNIT][DSP/DSP-001] - Dispatch command
			Given Performer registrered for command
			When Dispatching command
			Then Command performer should be performed

		@issue-6
		@unit
		Scenario: [UNIT][DSP/DSP-002] - Performer not exists for command
			When Dispatching command
			Then NotImplementedException should be thrown
		
		@issue-6
		@unit
		Scenario: [UNIT][DSP/DSP-003] - Dispatch query
			Given Performer registrered for query
			When Dispatching query
			Then Query performer should be performed

		@issue-6
		@unit
		Scenario: [UNIT][DSP/DSP-004] - Performer not exists for query
			When Dispatching query
			Then NotImplementedException should be thrown

	Rule: [DSP/AUT] - Authorize requests

		@issue-24
		@api
		Scenario: [API][DSP/AUT-001] - Request authorization
			When Dispatching authorized request
			Then Response should be
				| StatusCode |
				| 202        |

		@issue-24
		@api
		Scenario: [API][DSP/AUT-002] - Unathorized request
			When Dispatching unauthorized request
			Then Response should be
				| StatusCode |
				| 401        |

		@issue-24
		@api
		Scenario: [API][DSP/AUT-003] - Forbidden request
			When Dispatching forbidden request
			Then Response should be
				| StatusCode |
				| 403        |

		@issue-24
		@api
		Scenario: [API][DSP/AUT-004] - Allow anonymus
			When Dispatching allow anonymus request
			Then Response should be
				| StatusCode |
				| 202        |