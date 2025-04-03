import { useParams } from "react-router-dom";
import { useTodoGroupStore } from "./todoGroupStore";
import { Descriptions } from "antd";
import { useEffect } from "react";
import { TodoGroupService } from "./todoGroupService";
import TodoItemList from "../TodoItem/TodoItemList";

export const TodoGroupView = () => {
  const { currentTodoGroup, setTodoGroupCurrent } = useTodoGroupStore();
  const { listId } = useParams();

  if (!listId) throw new Error("listId is required");

  useEffect(() => {
    TodoGroupService.fetchTodoGroupById(listId).then((res) =>
      setTodoGroupCurrent(res.data, null)
    );
  }, [listId, setTodoGroupCurrent]);

  return (
    <>
      {currentTodoGroup && (
        <>
          <Descriptions column={1} bordered>
            <Descriptions.Item label="Name">
              {currentTodoGroup.title}
            </Descriptions.Item>
            <Descriptions.Item label="Colour">
              <div
                style={{
                  display: "inline-block",
                  width: 24,
                  height: 24,
                  borderRadius: "50%",
                  backgroundColor: currentTodoGroup.colour,
                  border: "1px solid #ccc",
                }}
              />
            </Descriptions.Item>
          </Descriptions>
          <div className="mt-4">
            <TodoItemList />
          </div>
        </>
      )}
    </>
  );
};
