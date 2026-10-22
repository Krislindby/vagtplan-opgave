//Oversætter. Guid bliver til string, DateOnly bliver til "2026-12-01".

export interface PlanningPeriod {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
}

export interface SavePlanningPeriod {
  name: string;
  startDate: string;
  endDate: string;
}

export interface Ward {
  id: string;
  name: string;
  color: string;
}

export interface SaveWard {
  name: string;
  color: string;
}

export interface StaffGroup {
  id: string;
  name: string;
}

export interface SaveStaffGroup {
  name: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
  isAdmin: boolean;
  isStaff: boolean;
}

export interface SaveUser {
  name: string;
  email: string;
  isAdmin: boolean;
  isStaff: boolean;
}

export interface Employment {
  id: string;
  userId: string;
  userName: string;
  wardId: string;
  wardName: string;
  staffGroupId: string;
  staffGroupName: string;
  hoursPerWeek: number;
  startDate: string;
  endDate: string;
}

export interface SaveEmployment {
  userId: string;
  wardId: string;
  staffGroupId: string;
  hoursPerWeek: number;
  startDate: string;
  endDate: string;
}