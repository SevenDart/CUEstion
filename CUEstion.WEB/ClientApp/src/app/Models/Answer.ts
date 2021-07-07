import {AppComment} from './AppComment';
import {User} from './User';

export interface Answer {
  id: number;
  text: string;
  rate: number;
  user: User;
  createTime: Date;
  updateTime: Date;
  comments: AppComment[];
}
