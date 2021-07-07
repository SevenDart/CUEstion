import {User} from './User';

export interface AppComment {
  id: number;
  text: string;
  rate: number;
  user: User;
  createTime: Date;
  updateTime: Date;
}
