export const isFemaleDoctorName = (name: string = ''): boolean => {
  const femaleNames = [
    'sara', 'nour', 'mona', 'hoda', 'layla', 'eman', 'reem', 'dalia', 'fatma', 'salma', 
    'rania', 'aya', 'noha', 'habiba', 'dina', 'marwa', 'yasmin', 'esraa', 'shaimaa', 'doaa', 
    'mariam', 'heba', 'riham', 'nada', 'nermin', 'nehal', 'omnia', 'samar', 'radwa', 'mai', 
    'sherin', 'ghada', 'inas', 'amal', 'manal', 'hala', 'sahar', 'amira', 'basma', 'naglaa', 
    'abeer', 'wafaa', 'nawal', 'soha', 'nagwa', 'zeinab', 'zainab', 'hania', 'faten', 'rasha',
    'إيمان', 'سارة', 'منى', 'هدى', 'ليلى', 'مريم', 'فاطمة', 'ريم', 'داليا', 'سلمى', 'رانيا',
    'آية', 'نهى', 'حبيبة', 'دينا', 'مروة', 'ياسمين', 'إسراء', 'شيماء', 'دعاء', 'هبة', 'ريهام',
    'ندى', 'نرمين', 'أمنية', 'سمر', 'رضوى', 'مي', 'شيرين', 'غادة', 'إيناس', 'أمل', 'منال',
    'هالة', 'سحر', 'أميرة', 'بسمة', 'نجلاء', 'عبير', 'وفاء', 'نوال', 'سها', 'نجوى', 'زينب'
  ];
  const lower = name.toLowerCase();
  return femaleNames.some(fn => lower.includes(fn));
};

export const getDoctorAvatarUrl = (fullName: string = '', index: number = 0): string => {
  const isFemale = isFemaleDoctorName(fullName);
  const category = isFemale ? 'women' : 'men';
  let hash = 0;
  for (let i = 0; i < fullName.length; i++) {
    hash = (hash << 5) - hash + fullName.charCodeAt(i);
    hash |= 0;
  }
  const avatarId = (Math.abs(hash + index) % 90) + 1;
  return `https://randomuser.me/api/portraits/${category}/${avatarId}.jpg`;
};
