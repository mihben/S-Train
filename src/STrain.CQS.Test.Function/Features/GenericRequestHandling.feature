Feature: GenericRequestHandling

Possible to recieve request via generic endpoint. The request type is determined based on the ___request-type___ header in ___'{namespace}.{requestType}, {assembly}'___ format. Assembly must not contain version information.

	Rule: Handle Command
		
		Handle commands by generic request handler. Configurations:
			- __Path__: configurable (default: /api)
			- __Method__: POST
			- __StatusCode__: 202 - Accepted
			
		@issue-14
		@api
		Scenario: Receive Command
			When Receiving command
			Then Command should be performed by performer