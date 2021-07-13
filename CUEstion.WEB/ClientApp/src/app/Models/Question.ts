import {AppComment} from './AppComment';
import {User} from './User';
import {Answer} from './Answer';

export interface Question {
  id: number;
  header: string;
  text: string;
  rate: number;
  createTime: Date;
  updateTime: Date;
  tags: string[];
  user: User;
  answers: Answer[];
  comments: AppComment[];
}
