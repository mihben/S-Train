Feature: Request

Supports __C__ommand __Q__uery __S__eparation pattern. The fundamental idea is that we should divide an object's methods into two sharply separated categories:
- Command
- Query

More information _(here)[https://martinfowler.com/bliki/CommandQuerySeparation.html]_

	Rule: Command
		Commands change the state of the system, but does not have return value.

		@issue-3
		@unit
		Scenario: Equals commands
			Given Command with E2DF8CA8-2ADE-42AD-B2EA-E033F0E74730 id
			And Command with E2DF8CA8-2ADE-42AD-B2EA-E033F0E74730 id
			When Comparing requests
			Then Should be equals

		@issue-3
		@unit
		Scenario: Different commands
			Given Command with E2DF8CA8-2ADE-42AD-B2EA-E033F0E74730 id
			And Command with 9901DB37-6747-43C2-BD24-F6C1F722A770 id
			When  Comparing requests
			Then Should not be equals

		@issue-3
		@unit
		Scenario: Null command
			Given 'A' command is null
			And Command with 9901DB37-6747-43C2-BD24-F6C1F722A770 id
			When  Comparing requests
			Then Should not be equals

	Rule: Query
		 Return a result and do not change the observable state of the system (are free of side effects).

		 @issue-3
		 @unit
		Scenario: Equals queries
			Given Query with E2DF8CA8-2ADE-42AD-B2EA-E033F0E74730 id
			And Query with E2DF8CA8-2ADE-42AD-B2EA-E033F0E74730 id
			When Comparing requests
			Then Should be equals

		@issue-3
		@unit
		Scenario: Different queries
			Given Query with E2DF8CA8-2ADE-42AD-B2EA-E033F0E74730 id
			And Query with 9901DB37-6747-43C2-BD24-F6C1F722A770 id
			When  Comparing requests
			Then Should not be equals

		@issue-3
		@unit
		Scenario: Null query
			Given 'A' query is null
			And Query with 9901DB37-6747-43C2-BD24-F6C1F722A770 id
			When  Comparing requests
			Then Should not be equals