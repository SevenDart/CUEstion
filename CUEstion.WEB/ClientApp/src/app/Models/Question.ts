import {AppComment} from './AppComment';
import {User} from './User';

export interface Question {
  id: number;
  header: string;
  text: string;
  rate: number;
  createTime: Date;
  updateTime: Date;
  tags: string[];
  user: User;
  comments: AppComment[];
}
