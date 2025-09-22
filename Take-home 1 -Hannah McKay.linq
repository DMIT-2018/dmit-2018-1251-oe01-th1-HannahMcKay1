<Query Kind="Statements">
  <Connection>
    <ID>0ed0b5b7-bd80-47ba-b2e8-6107389101e5</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>HANNAH\SQLEXPRESS06</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>StartTed-2025-Sept</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//Question 1:The Dean wants a clear, chronological lineup of every upcoming 
//club activity (starting January 1, 2025) that takes place somewhere other than the standard 
//“Scheduled Room,” and isn’t just the routine BTech Club meeting. This will feed into the new 
//“Get Out & Get Involved” social-media campaign and guide students toward 
//discovering fresh experiences on and off campus.

ClubActivities
	.Where(activity =>
		activity.StartDate >= new DateTime(2025, 1, 1) 
		&& activity.Name != "BTech Club Meeting"
		&& activity.CampusVenue.Location != "Scheduled Room")
	.OrderBy(activity => activity.StartDate)
	.Select(activity => new
	{
		activity.StartDate,
		Location = activity.CampusVenue.Location,
		Club = activity.Club.ClubName,
		Activity = activity.Name
	})
	.Dump();

//Question 2: generate a comprehensive overview of every program offered 
//across our schools that meets accreditation requirements. Your report will 
//translate each school code into a friendly name, tally how many 
//courses in each program are mandatory versus optional, and then highlight only those 
//programs that have at least twenty-two required courses, so department heads can 
//prioritize resourcing and scheduling decisions.

Programs
	.Where(program => program.ProgramCourses.Count(course => course.Required) >= 22)
	.OrderBy(program => program.ProgramName)
	.Select(program => new
	{
		School = program.SchoolCode == "SAMIT" ? "School of Advance Media and IT"
			   : program.SchoolCode == "SEET" ? "School of Electrical Engineering Technology"
			   : "Unknown",

		Program = program.ProgramName,
		RequiredCourseCount = program.ProgramCourses.Count(course => course.Required),
		OptionalCourseCount = program.ProgramCourses.Count(course => !course.Required)
	})
	.Dump();

//Question 3: 
//identify all non-Canadian students who have not yet made any tuition payments. 
//This allows your team to reach out proactively and assist them in completing 
//their registration. You’ll also provide each student’s home country and 
//indicate whether they’ve joined any campus clubs—to gauge their level of campus engagement.

Students
	.Where(student =>
		student.StudentPayments.Count() == 0 &&
		student.Countries.CountryName != "Canada")

	.OrderBy(student => student.LastName)
	.Select(student => new
	{
		student.StudentNumber,
		student.Countries.CountryName,
		FullName = student.FirstName + " " + student.LastName,
		ClubMembershipCount = student.ClubMembers.Count() == 0 ? "None" : student.ClubMembers.Count().ToString()
	})
	.Dump();

//Question 4:
//review all active instructors currently teaching classes this term. 
//You need a ranked list showing each instructor’s program affiliation, 
//their full name, and a simple workload category—so you can 
//balance teaching assignments and offer support where needed.

Employees
	.Where(employee =>
		employee.Position.Description == "Instructor" &&
		employee.ReleaseDate == null &&
		employee.ClassOfferings.Count() > 0)

	.OrderByDescending(employee => employee.ClassOfferings.Count())
	.ThenBy(employee => employee.LastName)
	.Select(employee => new
	{
		employee.Program.ProgramName,
		FullName = employee.FirstName + " " + employee.LastName,
		WorkLoad = employee.ClassOfferings.Count() > 24 ? "High"
				 : employee.ClassOfferings.Count() > 8 ? "Med"
				 : "Low"
	})
	.Dump();

//Question 5:
//Produce a snapshot of all clubs on campus. Your report will list each
//clubs faculty supervisor, membership size, and upcoming activity count--
//so you can recognize high-performing clubs and identity those that may need support.

Clubs
	.OrderByDescending(club => club.ClubMembers.Count())
	.Select(club => new
	{
		Supervisor = club.Employee == null ? "Unknown" : club.Employee.FirstName + " " + club.Employee.LastName,
		club.ClubName,
		MemberCount = club.ClubMembers.Count(),
		Activities = club.ClubActivities.Count() == 0 ? "None Schedule" : club.ClubActivities.Count().ToString()
	})
	.Dump();
