import {AppComment} from './AppComment';

export interface Question {
  id: number;
  header: string;
  text: string;
  rate: number;
  createTime: Date;
  updateTime: Date;
  tags: string[];
  comments: AppComment[];
}
