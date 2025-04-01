  export interface DoctorAvailability {
    id: number;
    dayOfWeek: DayOfWeek;
    startTime: string; // Using string because TimeSpan is not a native JS type
    endTime: string;
    doctorId: string;
  }
  
  export enum DayOfWeek {
    Sunday = 'Sunday',
    Monday = 'Monday',
    Tuesday = 'Tuesday',
    Wednesday = 'Wednesday',
    Thursday = 'Thursday',
    Friday = 'Friday',
    Saturday = 'Saturday'
  }
  